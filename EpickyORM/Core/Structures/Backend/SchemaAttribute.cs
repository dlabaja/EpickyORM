namespace EpickyORM.Core.Structures.Backend;

public class SchemaAttribute
{
    public string Name { get; }
    public Type Type { get; }
    public bool IsPk { get; }
    public bool IsRelation { get; }
    
    public SchemaAttribute(string name, Type type, bool isPk, bool isRelation)
    {
        Name = name;
        Type = type;
        IsPk = isPk;
        IsRelation = isRelation;
    }
}