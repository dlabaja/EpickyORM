namespace EpickyORM.Core.Structures.Backend;

// třída mapující hodnoty na parametry
// stačí ji předat některé z metod Driveru
public class ParameterKeeper
{
    private int _currentId = 1;
    // díky zápočtovýmu testu jsem se naučil správně používat increment
    public int GetNewId() => _currentId++;
    public string NullName => "pnull";

    private readonly Dictionary<string, (object? Value, string Name)> _paramMapper = new(); // value.ToString -> (value, paramName)

    // udělá si záznam o parametru (pokud neexistuje) a vrátí jméno parametru
    public string GetParamName(object? value)
    {
        if (value?.ToString() == null)
        {
            return "pnull";
        }

        var key = $"{value.GetType()}--{value}";
        if (_paramMapper.TryGetValue(key, out var val))
        {
            return val.Name;
        }
        _paramMapper.Add(key, (value, $"p{GetNewId()}"));
        return _paramMapper[key].Name;
    }

    public List<(object? Value, string Name)> Parameters => [.. _paramMapper.Values, (null, NullName)];
}