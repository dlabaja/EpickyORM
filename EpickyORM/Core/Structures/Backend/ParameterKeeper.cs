namespace EpickyORM.Core.Structures.Backend;

public class ParameterKeeper
{
    private int _currentId = 1;
    // díky zápočtovýmu testu jsem se naučil správně používat increment
    public int GetNewId() => _currentId++;

    private readonly Dictionary<object, string> _paramMapper = new();

    public string GetParamName(object value)
    {
        if (_paramMapper.TryGetValue(value, out var name))
        {
            return name;
        }
        _paramMapper.Add(value, $"p{GetNewId()}");
        return _paramMapper[value];
    }

    public List<(object Value, string Name)> Parameters => [.. _paramMapper.Keys.Zip(_paramMapper.Values)];
}