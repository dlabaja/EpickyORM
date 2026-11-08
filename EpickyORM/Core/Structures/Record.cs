namespace EpickyORM.Core.Structures;

public class Record
{
    public object Instance { get; }
    public Type Type { get; }

    public Record(object instance, Type type)
    {
        Instance = instance;
        Type = type;
    }
}