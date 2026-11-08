namespace EpickyORM.Core.Structures;

public class DynamicValue
{
    public object Value { get; }
    public Type Type { get; }

    public DynamicValue(object value, Type type)
    {
        Value = value;
        Type = type;
    }
}