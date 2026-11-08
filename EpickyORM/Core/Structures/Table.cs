using System.Reflection;
using EpickyORM.Core.Attributes;
using EpickyORM.Core.Enum;
using EpickyORM.Core.Utils;

namespace EpickyORM.Core.Structures;

public class Table
{
    public string Name { get; }
    public Schema Schema { get; }
    public Type Type { get; }
    private Dictionary<SchemaAttribute, Func<object, object?>> _getters = new();
    private List<Record> _records = [];
    
    public Table(Type type)
    {
        Type = type;
        Name = Type.Name;
        Schema = GenerateSchema();
    }

    private Schema GenerateSchema()
    {
        var properties = Type.GetProperties().Where(x => x.GetCustomAttribute<Ignore>() == null);
        var fields = Type.GetFields().Where(x => x.GetCustomAttribute<Include>() != null);

        var schemaAttributes = new List<SchemaAttribute>();
        foreach (var property in properties)
        {
            var attribute = new SchemaAttribute(property.Name.ToSnakeCase(), property.PropertyType);
            schemaAttributes.Add(attribute);
            _getters.Add(attribute, obj => property.GetValue(obj));
        }

        foreach (var field in fields)
        {
            var attribute = new SchemaAttribute(field.Name.ToSnakeCase(), field.FieldType);
            schemaAttributes.Add(attribute);
            _getters.Add(attribute, obj => field.GetValue(obj));
        }

        return new Schema(schemaAttributes);
    }

    public List<(SchemaAttribute x, object?)> GetRecordValues(Record record)
    {
        if (!FulfillsSchema(record))
        {
            throw new Exception($"Cannot get Record value - Record of type {record.Type} doesn't fullfill schema of {Type}");
        }

        return Schema.SchemaAttributes.Select(x => (x, _getters[x].Invoke(record.Instance))).ToList();
    }

    public bool FulfillsSchema(Record record)
    {
        return Type == record.Type;
    }

    public void Add(Record record)
    {
        _records.Add(record);
    }

    public List<Record> GetRecords()
    {
        return _records;
    }

    public void Flush()
    {
        foreach (var record in _records)
        {
            record.ResetAction();
        }
    }
}