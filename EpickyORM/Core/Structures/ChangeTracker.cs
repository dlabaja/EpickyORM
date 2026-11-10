namespace EpickyORM.Core.Structures;

public class ChangeTracker
{
    private Dictionary<Type, Table> _tables = new();

    public List<Table> Tables => _tables.Values.ToList();

    public void RegisterTable(Table table)
    {
        _tables.Add(table.Type, table);
    }
    
    public void Add(Record record)
    {
        if (!_tables.ContainsKey(record.Type))
        {
            throw new Exception($"There is no table for record of type {record.Type}");
        }
        _tables[record.Type].Add(record);
    }

    public void Flush()
    {
        foreach (var table in _tables.Values)
        {
            table.Flush();   
        }
    }
}