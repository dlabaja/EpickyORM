using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

public class QueryAttributeNames : QueryExpression
{
    public List<QueryAttributeName> Names { get; }
    
    public QueryAttributeNames(List<QueryAttributeName> names)
    {
        Names = names;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetAttributeNames(this);
    }
}