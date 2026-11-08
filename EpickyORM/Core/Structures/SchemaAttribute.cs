namespace EpickyORM.Core.Structures;

public class SchemaAttribute
{
    public string Name { get; }
    public Type Type { get; }

    public SchemaAttribute(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}