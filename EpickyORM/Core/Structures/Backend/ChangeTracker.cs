namespace EpickyORM.Core.Structures.Backend;

public interface ITable
{
    void Flush();
}

// tahle třída je důvod proč mám radši TypeScript, striktní typy jsou strašně omezující, pomoc
public class ChangeTracker
{
    private Dictionary<Type, object> _tables = new();

    public List<object> Tables => [.. _tables.Values];

    public void RegisterTable(object table)
    {
        _tables.Add(table.GetType(), table);
    }
    
    public void Add<T>(Record<T> record)
    {
        if (!_tables.ContainsKey(record.GetType()))
        {
            throw new Exception($"There is no table for record of type {record.GetType()}");
        }
        GetTable<T>().Add(record);
    }

    public Table<T> GetTable<T>()
    {
        return (Table<T>)_tables[typeof(T)];
    }

    public void Flush()
    {
        foreach (var table in _tables.Values)
        {
            // fake rozhraní
            var t = (ITable)table;
            t.Flush();   
        }
    }
}