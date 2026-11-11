using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

public class QueryParameter : QueryExpression
{
    public object? Value { get; }
    public int Id { get; }
    public string Name => $"p{Id}";
    
    public QueryParameter(object? value, int id)
    {
        Value = value;
        Id = id;
    }

    public override string Parse(CommandGenerator generator)
    {
        return generator.GetParameter(this);
    }
}