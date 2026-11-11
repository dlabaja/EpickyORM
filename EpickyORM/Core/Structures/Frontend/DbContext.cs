using System.Reflection;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;

namespace EpickyORM.Core.Structures.Frontend;

public abstract class DbContext
{
    private readonly ChangeTracker _changeTracker;
    private bool _triedCreatingTables;

    // poprvé používám internal, díky tomu můžu Driver nastavit z databáze ale nikdo mimo namespace se k němu nedostane
    internal DatabaseDriver Driver { get; private set; }

    protected DbContext()
    {
        _changeTracker = new ChangeTracker();
    }

    internal void Init(DatabaseDriver driver)
    {
        Driver = driver;
        CheckDbSets();
        CreateDbSets();
        TryCreatingTables();
    }

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
        var invalidProperties = properties
            .Where(x => 
                !x.PropertyType.IsGenericType || x.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>)); // bez genericTypeDef by se to muselo rovat DbSet<T>
        if (invalidProperties.Any())
        {
            throw new Exception($"Class {GetType()} can only have properties of DbSet<class> type.");
        }

        foreach (var property in properties)
        {
            var tableType = property.PropertyType.GetGenericArguments()[0];
            var dbSetType = typeof(DbSet<>).MakeGenericType(tableType);

            var tableGenericType = typeof(Table<>).MakeGenericType(tableType);
            var table = Activator.CreateInstance(tableGenericType);
            _changeTracker.RegisterTable(table);

            // tvorba instance, uložím ji do Contextu
            var instance = Activator.CreateInstance(dbSetType, table, Driver, _changeTracker);
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

    private void TryCreatingTables()
    {
        if (_triedCreatingTables)
        {
            return;
        }

        foreach (var table in _changeTracker.Tables)
        {
            Driver.TryCreateTable(table);
        }

        _triedCreatingTables = true;
    }

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
            table.SetValue(table.Schema.GetPrimaryKeyAttribute(), record, startId);
            startId++;
        }
    }

    private void ProcessRecordsToUpdate<T>(DatabaseDriver driver, Table<T> table, List<Record<T>> _records)
    {
        var records = _records.Where(x => x.RecordAction == RecordAction.ADD).ToList();
        if (records.Count == 0)
        {
            return;
        }
        driver.Upsert(table, records, new ParameterKeeper());
    }

    private void ProcessRecordsToRemove<T>(DatabaseDriver driver, Table<T> table, List<Record<T>> _records)
    {
        var records = _records.Where(x => x.RecordAction == RecordAction.ADD).ToList();
        if (records.Count == 0)
        {
            return;
        }
        driver.Delete(table, records, new ParameterKeeper());
    }

    // nevím jak se mi to povedlo, ale po asi hodině jsem znásilnil C# natolik že mi dovolil mít dynamický tabulky v ChangeTrackeru
    public void SaveChanges()
    {
        foreach (var table in _changeTracker.Tables)
        {
            ProcessRecordsToAdd(Driver, table, table.GetRecords());
            ProcessRecordsToUpdate(Driver, table, table.GetRecords());
            ProcessRecordsToRemove(Driver, table, table.GetRecords());
        }

        _changeTracker.Flush();
    }
}