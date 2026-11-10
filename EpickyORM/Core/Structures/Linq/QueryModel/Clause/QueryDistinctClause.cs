using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryDistinctClause : QueryExpression
{
    
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetDistinctClause(this);
    }
}