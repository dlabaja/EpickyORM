using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

public class QueryAttributeName : QueryExpression
{
    public string Name { get; }
    
    public QueryAttributeName(string name)
    {
        Name = name;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetAttributeName(this);
    }
}