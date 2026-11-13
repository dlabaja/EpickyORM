using System.Reflection;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq;

namespace EpickyORM.Core.Structures.Frontend;

// množina/kolekce záznamů
public class DbSet<T> : SqlQueryable<T>
{
    private readonly ChangeTracker _changeTracker;

    public DbSet(Table<T> table, DatabaseDriver driver, ChangeTracker changeTracker) : base(new SqlQueryProvider<T>(table, driver))
    {
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

    private object? GetPrimaryKeyValue(T instance)
    {
        return typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttribute<Attributes.PrimaryKey>() != null)?.GetValue(instance);
    }
    
    public void Add(T instance)
    {
        if (instance == null)
        {
            return;
        }

        var pk = GetPrimaryKeyValue(instance);
        // klíč může být i int nebo teoreticky i string. C# ho ale vnímá jako objekt (default = null), proto ho ještě musím porovnat s defaultní hodnotou konkrétního typu
        // defaultní hodnota = přidej, jinak zkus přidat/updatni
        var isDefault = pk == null || pk.Equals(Activator.CreateInstance(pk.GetType()));
        var record = new Record<T>(instance, isDefault ? RecordAction.ADD : RecordAction.UPDATE);
        _changeTracker.Add(record);
    }

    public void Remove(T instance)
    {
        if (instance == null)
        {
            return;
        }

        var pk = GetPrimaryKeyValue(instance);
        if (pk != null)
        {
            _changeTracker.Add(new Record<T>(instance, RecordAction.REMOVE));
        }
    }
}