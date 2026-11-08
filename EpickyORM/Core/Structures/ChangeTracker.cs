namespace EpickyORM.Core.Structures;

public class ChangeTracker
{
    private Dictionary<Type, Table> _tables = new();

    public List<Table> Tables => _tables.Values.ToList();

    public ChangeTracker(List<Type> types)
    {
        GenerateTables(types);
    }

    private void GenerateTables(List<Type> types)
    {
        foreach (var type in types)
        {
            _tables.Add(type, new Table(type));
        }
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