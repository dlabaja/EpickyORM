namespace EpickyORM.Core.Structures;

public class Table
{
    public string Name { get; }
    public List<TableAttribute> TableAttributes { get; }
}