using EpickyORM.Core.Structures.Linq.QueryModel.Clause;

namespace EpickyORM.Core.Structures.Linq.QueryModel;

public class QueryRoot
{
    public QueryWhereClause WhereClause { get; }
    
    public QueryRoot(QueryWhereClause whereClause)
    {
        WhereClause = whereClause;
    }
}