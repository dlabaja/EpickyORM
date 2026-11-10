using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryOrderByClause : QueryExpression
{
    public QueryAttributeNames AttributeNames { get; }
    public OrderByDirection OrderDirection { get; }
    
    public QueryOrderByClause(QueryAttributeNames attributeNames, OrderByDirection orderDirection)
    {
        AttributeNames = attributeNames;
        OrderDirection = orderDirection;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetOrderByClause(this);
    }
}