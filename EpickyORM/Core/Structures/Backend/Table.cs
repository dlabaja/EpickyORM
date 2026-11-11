using System.Reflection;
using EpickyORM.Core.Attributes;
using EpickyORM.Core.Utils;

namespace EpickyORM.Core.Structures.Backend;

public class Table<T>
{
    public string Name { get; }
    public Schema Schema { get; }
    private readonly Dictionary<SchemaAttribute, Func<object, object?>> _getters = new();
    private readonly Dictionary<SchemaAttribute, Action<object, object>> _setters = new();
    private readonly List<Record<T>> _records = [];
    
    // tohle by teoreticky mohla být generická třída, ale pak bych je nemohl spojovat do kolekcí (každá tabulka má jinej typ)
    public Table()
    {
        Name = typeof(T).Name;
        Schema = GenerateSchema();
    }

    private Schema GenerateSchema()
    {
        var properties = typeof(T).GetProperties().Where(x => x.GetCustomAttribute<Ignore>() == null);
        var fields = typeof(T).GetFields().Where(x => x.GetCustomAttribute<Include>() != null);
        var primaryKey = typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttribute<PrimaryKey>() != null);
        if (primaryKey == null)
        {
            throw new Exception("No primary key found");
        }
        
        var schemaAttributes = new List<SchemaAttribute>();
        foreach (var property in properties)
        {
            var attribute = new SchemaAttribute(property.Name.ToSnakeCase(), property.PropertyType, property.Name == primaryKey.Name);
            schemaAttributes.Add(attribute);
            _getters.Add(attribute, obj => property.GetValue(obj));
            _setters.Add(attribute, (obj, val) => property.SetValue(obj, val));
        }

        foreach (var field in fields)
        {
            var attribute = new SchemaAttribute(field.Name.ToSnakeCase(), field.FieldType, false);
            schemaAttributes.Add(attribute);
            _getters.Add(attribute, obj => field.GetValue(obj));
            _setters.Add(attribute, (obj, val) => field.SetValue(obj, val));
        }

        return new Schema(schemaAttributes);
    }

    public List<(SchemaAttribute x, object?)> GetRecordValues(Record<T> record)
    {
        return [.. Schema.SchemaAttributes.Select(x => (x, _getters[x].Invoke(record.Instance)))];
    }

    public object? GetValue(SchemaAttribute schemaAttribute, Record<T> record)
    {
        return _getters[schemaAttribute].Invoke(record.Instance);
    }

    public void SetValue(SchemaAttribute schemaAttribute, Record<T> record, object value)
    {
        _setters[schemaAttribute].Invoke(record.Instance, value);
    }

    public void Add(Record<T> record)
    {
        _records.Add(record);
    }

    public List<Record<T>> GetRecords()
    {
        return _records;
    }

    public object GetRecordPk(Record<T> record)
    {
        return GetValue(Schema.GetPrimaryKeyAttribute(), record) ?? throw new Exception("This record has no primary key");
    }

    public void Flush()
    {
        foreach (var record in _records)
        {
            record.ResetAction();
        }
    }
}