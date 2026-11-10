using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;

namespace EpickyORM.Core.Structures.Linq.QueryModel;

public class QueryRoot : QueryExpression
{
    public QueryWhereClause? WhereClause { get; set; }
    public QueryOrderByClause? OrderByClause { get; set; }
    public QueryDistinctClause? DistinctClause { get; set; }
    public QueryLimitClause? LimitClause { get; set; }
    public QueryOffsetClause? OffsetClause { get; set; }
    private Table _table;

    public QueryRoot(Table table)
    {
        _table = table;
    }

    public override string Parse(CommandGenerator generator)
    {
        return generator.Select(_table, this);
    }
}