namespace EpickyORM.Core.Structures.Backend;

public class ParameterKeeper
{
    private int _currentId = 1;
    // díky zápočtovýmu testu jsem se naučil správně používat increment
    public int GetNewId() => _currentId++;
    
    public void Add(string name, object? value)
    {
        Parameters.Add((name, value));
    }

    public List<(string Name, object Value)> Parameters { get; } = [];
}