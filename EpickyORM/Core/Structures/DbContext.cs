using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures;

public abstract class DbContext
{
    private readonly ChangeTracker _changeTracker;
    private bool _triedCreatingTables;

    // poprvé používám internal, díky tomu můžu Driver nastavit z databáze ale nikdo mimo namespace se k němu nedostane
    internal DatabaseDriver? Driver { get; set; }

    protected DbContext()
    {
        _changeTracker = new ChangeTracker();
        CheckDbSets();
        CreateDbSets();
        TryCreatingTables();
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

            var tableType = property.PropertyType.GetGenericArguments()[0];
            var table = new Table(tableType);
            _changeTracker.RegisterTable(table);

            // tvorba instance, uložím ji do Contextu
            var instance = Activator.CreateInstance(dbSet, table, Driver, _changeTracker);
            property.SetValue(this, instance);
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
            Driver?.TryCreateTable(table);
        }

        _triedCreatingTables = true;
    }

    private void ProcessRecordsToAdd(DatabaseDriver driver, Table table, List<Record> records)
    {
        driver.Insert(table, records, out var lastNewId);
        var startId = lastNewId - (records.Count - 1);
        foreach (var record in records)
        {
            table.SetValue(table.Schema.GetPrimaryKeyAttribute(), record, startId);
            startId++;
        }
    }

    private void ProcessRecordsToUpdate(DatabaseDriver driver, Table table, List<Record> records)
    {
        driver.Update(table, records);
    }

    private void ProcessRecordsToRemove(DatabaseDriver driver, Table table, List<Record> records)
    {
        driver.Delete(table, records);
    }

    public void SaveChanges()
    {
        if (Driver == null)
        {
            throw new Exception(
                $"No Database driver found - was the context {GetType()} registered using Database constructor?");
        }

        foreach (var table in _changeTracker.Tables)
        {
            ProcessRecordsToAdd(Driver, table,
                [.. table.GetRecords().Where(x => x.RecordAction == RecordAction.ADD)]);
            ProcessRecordsToUpdate(Driver, table,
                [.. table.GetRecords().Where(x => x.RecordAction == RecordAction.UPDATE)]);
            ProcessRecordsToRemove(Driver, table,
                [.. table.GetRecords().Where(x => x.RecordAction == RecordAction.REMOVE)]);
        }

        _changeTracker.Flush();
    }
}