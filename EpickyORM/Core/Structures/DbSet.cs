using System.Reflection;
using EpickyORM.Core.Enum;

namespace EpickyORM.Core.Structures;

// množina/kolekce záznamů
public class DbSet<T>
{
    // inicializuje se přes reflexi v DbContext
    private ChangeTracker _changeTracker;

    public DbSet()
    {
        CheckPrimaryKey();
        if (_changeTracker == null)
        {
            throw new Exception($"DbSet<{typeof(T)}> has to be inside class derived from DbContext");
        }
    }

    private void CheckPrimaryKey()
    {
        var pkAttr = typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttribute<Attributes.PrimaryKey>() != null);
        if (pkAttr == null)
        {
            throw new Exception(
                $"Class {typeof(T)} doesn't have a PrimaryKey attribute. Please add one above one of its properties.");
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

        var record = new Record(instance, instance.GetType());
        if (pk == null)
        {
            _changeTracker.Add(record, RecordAction.ADD);
            return;
        }
        
        _changeTracker.Add(record, RecordAction.UPDATE);
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
            var record = new Record(instance, instance.GetType());
        }
    }
}