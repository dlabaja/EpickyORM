namespace EpickyORM.Core.Structures.Backend;

public class SchemaAttribute
{
    public string Name { get; }
    public Type Type { get; }
    public bool IsPk { get; } 

    public SchemaAttribute(string name, Type type, bool isPk)
    {
        Name = name;
        Type = type;
        IsPk = isPk;
    }
}