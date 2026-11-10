using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryLimitClause : QueryExpression
{
    public QueryParameter Limit { get; }
    
    public QueryLimitClause(QueryParameter limit)
    {
        Limit = limit;
    }

    public override string Parse(CommandGenerator generator)
    {
        return generator.GetLimitClause(this);
    }
}