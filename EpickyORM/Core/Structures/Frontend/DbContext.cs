using System.Reflection;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;

namespace EpickyORM.Core.Structures.Frontend;

// Kontext udržující schéma celé databáze (skrz DbSety), zpracovává obsah ChangeTrackeru
public abstract class DbContext
{
    private readonly ChangeTracker _changeTracker;

    private DatabaseDriver _driver { get; set; }

    protected DbContext()
    {
        _changeTracker = new ChangeTracker();
    }

    // inituje se z Database
    // mohl by to být konstruktor, ale vytvářet tabulky nad databází v konstruktoru není moc dobrej pattern
    internal void Init(DatabaseDriver driver)
    {
        _driver = driver;
        CheckDbSets();
        CreateDbSets();
        TryCreatingTables();
    }

    // zkontroluje jestli má třída dědící z DbContext jenom DbSety
    private void CheckDbSets()
    {
        var properties = GetType().GetProperties();
        var invalidProperties = properties
            .Where(x => !x.PropertyType.IsGenericType || x.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>)); // bez genericTypeDef bych to porovnával s DbSet<T> != DbSet<> -> vždycky true
        if (invalidProperties.Any())
        {
            throw new Exception($"Class {GetType()} can only have properties of DbSet<class> type.");
        }
    }

    private void CreateDbSets()
    {
        var properties = GetType().GetProperties();

        foreach (var property in properties)
        {
            // zjistím 
            var tableType = property.PropertyType.GetGenericArguments()[0];
            
            // vytvořím a zaregistruju tabulku
            var tableGenericType = typeof(Table<>).MakeGenericType(tableType);
            var table = Activator.CreateInstance(tableGenericType)!;
            _changeTracker.RegisterTable(table);
            
            // vytvořím DbSet instanci a předám jí odkaz na tabulku, changeTracker a driver (kvůli LINQ)
            var dbSetType = typeof(DbSet<>).MakeGenericType(tableType);
            var instance = Activator.CreateInstance(dbSetType, table, _driver, _changeTracker);

            // mapování instance DbSetu do properties v kontextu
            if (property.CanWrite) // má setter
            {
                property.SetValue(this, instance);
            }
            else
            {
                // https://stackoverflow.com/questions/68354413/set-property-of-instance-without-setter
                var setter = GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                setter?.SetValue(this, instance);
            }
        }
    }

    // vytvoří tabulku v DB pokud ještě neexistuje 
    private void TryCreatingTables()
    {
        foreach (var table in _changeTracker.Tables)
        {
            // musím používat reflexi, z těch typů zešílím
            var entityType = table.GetType().GetGenericArguments()[0];
            typeof(DatabaseDriver)
                .GetMethod(nameof(DatabaseDriver.TryCreateTable))!
                .MakeGenericMethod(entityType)
                .Invoke(_driver, new object[] { table });
        }
    }
    
    // nechávám stejný parametry abych nemusel přepisovat tu reflexi dole, stejně je to jedno
    // projde všechno rekordy v paměti u kterých prý nebyly změny, porovná aktuální s původním hashem a případně je nastaví na Edited
    private void ProcessRecordsToCheck<T>(DatabaseDriver driver, Table<T> table, List<Record<T>> _records)
    {
        var records = _records.Where(x => x.RecordAction == RecordAction.NONE).ToList();
        foreach (var record in records)
        {
            if (record.Instance == null)
            {
                continue;
            }
            
            if (_changeTracker.ObjectChanged(record.Instance))
            {
                record.RecordAction = RecordAction.UPDATE;
            }
        }
    }

    // btw v těhle dynamicky invokovaných metodách nefungují breakpointy, yay
    // všechny records s ADD mají pk = default
    // přidá záznam do DB a změní mu pk (podle toho co DB vygenerovala)
    private void ProcessRecordsToAdd<T>(DatabaseDriver driver, Table<T> table, List<Record<T>> _records)
    {
        var records = _records.Where(x => x.RecordAction == RecordAction.ADD).ToList();
        if (records.Count == 0)
        {
            return;
        }
        driver.Insert(table, records, new ParameterKeeper(), out var lastNewId);
        var startId = lastNewId - (records.Count - 1);
        foreach (var record in records)
        {
            table.SetValue(table.Schema.GetPrimaryKeyAttribute(), record.Instance, startId);
            startId++;
        }
    }

    // udělá nad záznamy UPSERT
    // záznamy už mají nastavené pk 
    private void ProcessRecordsToUpdate<T>(DatabaseDriver driver, Table<T> table, List<Record<T>> _records)
    {
        var records = _records.Where(x => x.RecordAction == RecordAction.UPDATE).ToList();
        if (records.Count == 0)
        {
            return;
        }
        driver.Upsert(table, records, new ParameterKeeper());
    }

    // smaže záznam z DB a tabulky v paměti (a tím i z trackeru)
    private void ProcessRecordsToRemove<T>(DatabaseDriver driver, Table<T> table, List<Record<T>> _records)
    {
        var records = _records.Where(x => x.RecordAction == RecordAction.REMOVE).ToList();
        if (records.Count == 0)
        {
            return;
        }
        driver.Delete(table, records, new ParameterKeeper());
        foreach (var record in records)
        {
            table.Remove(record);
        }
    }

    // nevím jak se mi to povedlo, ale po asi hodině jsem znásilnil C# natolik že mi dovolil mít dynamický tabulky v ChangeTrackeru
    // dynamicky si načtu records z Table a pak je pošlu to metod které si je podle actionu roztřídí a zpracují
    public void SaveChanges()
    {
        foreach (var table in _changeTracker.Tables)
        {
            // další znásilnění type systému, čekám soud v Haagu
            var tableType = table.GetType();
            var entityType = tableType.GetGenericArguments()[0];
            var getRecordsMethod = tableType.GetMethod(nameof(Table<>.GetRecords))!;
            var records = getRecordsMethod.Invoke(table, null);

            InvokeProcessMethod(nameof(ProcessRecordsToCheck), entityType, table, records);
            InvokeProcessMethod(nameof(ProcessRecordsToAdd), entityType, table, records);
            InvokeProcessMethod(nameof(ProcessRecordsToUpdate), entityType, table, records);
            InvokeProcessMethod(nameof(ProcessRecordsToRemove), entityType, table, records);
        }

        _changeTracker.Flush();
    }

    // prasárna pro typy
    private void InvokeProcessMethod(string methodName, Type entityType, object table, object records)
    {
        var method = typeof(DbContext)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)?
            .MakeGenericMethod(entityType);

        method?.Invoke(this, [_driver, table, records]);
    }
}