using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enum;

namespace EpickyORM.Core.Structures;

public abstract class DbContext
{
    private readonly ChangeTracker _changeTracker;
    private bool _triedCreatingTables;

    // poprvé používám internal, díky tomu můžu Driver nastavit z databáze ale nikdo mimo namespace se k němu nedostane
    internal DatabaseDriver? Driver { get; set; }

    protected DbContext()
    {
        CheckDbSets();
        CreateDbSets();

        _changeTracker =
            new ChangeTracker(GetType().GetProperties()
                .Select(x => x.PropertyType.GetGenericArguments()[0])
                .ToList());
        
        RegisterTracker();
    }

    private void CheckDbSets()
    {
        var properties = GetType().GetProperties();
        var invalidProperties = properties
            .Where(x => x.PropertyType != typeof(DbSet<>));
        if (invalidProperties.Any())
        {
            throw new Exception($"Class {GetType()} can only have properties of DbSet<class> type.");
        }
    }

    private void CreateDbSets()
    {
        var properties = GetType().GetProperties();
        var invalidProperties = properties
            .Where(x => !x.PropertyType.IsGenericType || x.PropertyType != typeof(DbSet<>));
        if (invalidProperties.Any())
        {
            throw new Exception($"Class {GetType()} can only have properties of DbSet<class> type.");
        }

        foreach (var property in properties)
        {
            var type = property.PropertyType.GetGenericArguments()[0];
            var dbSet = typeof(DbSet<>).MakeGenericType(type);
            // tvorba instance, uložím ji do Contextu
            var instance = Activator.CreateInstance(dbSet);
            property.SetValue(this, instance);
        }
    }

    private void RegisterTracker()
    {
        // this vrací aktuální instanci -> potomka téhle třídy
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var dbSetInstance = property.GetValue(this);
            var field = property.PropertyType.GetFields().FirstOrDefault(x => x.Name == "_changeTracker");
            if (field == null)
            {
                continue;
            }

            field.SetValue(dbSetInstance, _changeTracker);
        }
    }

    public void SaveChanges()
    {
        if (Driver == null)
        {
            throw new Exception(
                $"No Database driver found - was the context {GetType()} registered using Database constructor?");
        }

        if (!_triedCreatingTables)
        {
            foreach (var table in _changeTracker.Tables)
            {
                Driver.TryCreateTable(table);
            }
            _triedCreatingTables = true;
        }
        
        foreach (var table in _changeTracker.Tables)
        {
            Driver.Insert(table, table.GetRecords().Where(x => x.RecordAction == RecordAction.ADD).ToList());
        }
        _changeTracker.Flush();
    }
}