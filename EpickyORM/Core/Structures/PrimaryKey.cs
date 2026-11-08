namespace EpickyORM.Core.Structures;

public class PrimaryKey : DynamicValue
{
    private readonly Type[] _allowedTypes = [typeof(int), typeof(long)];

    public PrimaryKey(object value, Type type) : base(value, type)
    {
        if (!_allowedTypes.Contains(type))
        {
            throw new Exception($"Primarky key cannot be of type of {type}");
        }
    }

    public long GetValue()
    {
        // zřejmě inspirace F#
        return Value switch
        {
            int i => i,
            long i => i,
            // nemělo by se stát pokud to nezapomenu aktualizovat
            _ => throw new Exception($"Primarky key cannot be of type of {Type}")
        };
    }
}