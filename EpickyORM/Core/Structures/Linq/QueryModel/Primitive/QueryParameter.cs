using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Backend;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

public class QueryParameter : QueryExpression
{
    public object? Value { get; }
    private readonly ParameterKeeper _keeper;
    
    public QueryParameter(object? value, ParameterKeeper keeper)
    {
        Value = value;
        _keeper = keeper;
    }

    public override string Parse(CommandGenerator generator)
    {
        if (Value == null)
        {
            return "";
        }
        return generator.GetParameter(Value, _keeper);
    }
}