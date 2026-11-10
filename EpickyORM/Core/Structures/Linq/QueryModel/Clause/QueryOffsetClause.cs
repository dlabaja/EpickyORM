using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryOffsetClause : QueryExpression
{
    public QueryParameter OffsetCount { get; }
    
    public QueryOffsetClause(QueryParameter offsetCount)
    {
        OffsetCount = offsetCount;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetOffsetClause(this);
    }
}