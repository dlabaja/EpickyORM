using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryOffsetClause : QueryExpression
{
    public QueryParameter OffsetCount { get; }
    private readonly ParameterKeeper _keeper;
    
    public QueryOffsetClause(QueryParameter offsetCount, ParameterKeeper keeper)
    {
        OffsetCount = offsetCount;
        _keeper = keeper;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetOffsetClause(this, _keeper);
    }
}