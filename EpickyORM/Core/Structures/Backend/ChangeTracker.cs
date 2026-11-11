using EpickyORM.Core.Enums;

namespace EpickyORM.Core.Structures.Backend;

// tahle třída je důvod proč mám radši TypeScript, striktní typy jsou strašně omezující, pomoc
public class ChangeTracker
{
    private Dictionary<Type, dynamic> _tables = new();

    public List<dynamic> Tables => [.. _tables.Values];

    public void RegisterTable(object table)
    {
        _tables.Add(table.GetType().GenericTypeArguments[0], table);
    }
    
    public void Add<T>(Record<T> record)
    {
        if (!_tables.Values.Select(x => x.GetType().GenericTypeArguments[0]).Contains(typeof(T)))
        {
            throw new Exception($"There is no table for record of type {typeof(T)}");
        }
        GetTable<T>().Add(record);
    }

    public Table<T> GetTable<T>()
    {
        return (Table<T>)_tables[typeof(T)];
    }

    public List<Record<T>> GetRecords<T>(RecordAction action)
    {
        return GetTable<T>().GetRecords().Where(x => x.RecordAction == action).ToList();
    }

    public void Flush()
    {
        foreach (var table in Tables)
        {
            var tableType = table.GetType();
            var method = tableType.GetMethod("Flush")!;
            method.Invoke(table, null);
        }
    }
}