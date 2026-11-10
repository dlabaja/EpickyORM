using System.Reflection;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Linq;

namespace EpickyORM.Core.Structures;

// množina/kolekce záznamů
public class DbSet<T> : SqlQueryable<T>
{
    private Table _table;
    private DatabaseDriver _driver;
    private readonly ChangeTracker _changeTracker;

    public DbSet(Table table, DatabaseDriver driver, ChangeTracker changeTracker) : base(new SqlQueryProvider(table, driver))
    {
        _table = table;
        _driver = driver;
        _changeTracker = changeTracker;
        CheckPrimaryKey();
        if (_changeTracker == null)
        {
            throw new Exception($"DbSet<{typeof(T)}> has to be inside class derived from DbContext");
        }
    }

    private void CheckPrimaryKey()
    {
        var pkAttr = typeof(T).GetProperties().Where(x => x.GetCustomAttribute<Attributes.PrimaryKey>() != null);
        if (pkAttr == null)
        {
            throw new Exception(
                $"Class {typeof(T)} doesn't have a PrimaryKey attribute. Please add one above one of its properties.");
        }

        if (pkAttr.Count() > 1)
        {
            throw new Exception(
                $"Class {typeof(T)} has too many primary keys.");
        }
    }

    private PrimaryKey? GetPrimaryKey(T instance)
    {
        var value = typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttribute<Attributes.PrimaryKey>() != null)
            ?.GetValue(instance);
        return value != null ? new PrimaryKey(value, value.GetType()) : null;
    }

    public void Add(T instance)
    {
        if (instance == null)
        {
            return;
        }

        var pk = GetPrimaryKey(instance)?.GetValue();

        var record = new Record(instance, instance.GetType(), pk == null ? RecordAction.ADD : RecordAction.UPDATE);
        _changeTracker.Add(record);
    }

    public void Remove(T instance)
    {
        if (instance == null)
        {
            return;
        }

        var pk = GetPrimaryKey(instance)?.GetValue();
        if (pk != null)
        {
            _changeTracker.Add(new Record(instance, instance.GetType(), RecordAction.REMOVE));
        }
    }
}