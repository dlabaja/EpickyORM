using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryLimitClause : QueryExpression
{
    public QueryParameter Limit { get; }
    private readonly ParameterKeeper _keeper;
    
    public QueryLimitClause(QueryParameter limit, ParameterKeeper keeper)
    {
        Limit = limit;
        _keeper = keeper;
    }

    public override string Parse(CommandGenerator generator)
    {
        return generator.GetLimitClause(this, _keeper);
    }
}