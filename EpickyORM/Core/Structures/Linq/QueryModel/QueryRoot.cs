using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;

namespace EpickyORM.Core.Structures.Linq.QueryModel;

public class QueryRoot : QueryExpression
{
    public QueryWhereClause? WhereClause { get; set; }

    public override string Parse(CommandGenerator generator)
    {
        return generator.Select(this);
    }
}