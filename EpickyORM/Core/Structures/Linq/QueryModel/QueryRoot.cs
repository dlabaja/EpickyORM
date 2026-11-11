using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;

namespace EpickyORM.Core.Structures.Linq.QueryModel;

public class QueryRoot<T> : QueryExpression
{
    public ParameterKeeper Keeper = new ParameterKeeper();
    public QueryWhereClause? WhereClause { get; set; }
    public QueryOrderByClause? OrderByClause { get; set; }
    public QueryDistinctClause? DistinctClause { get; set; }
    public QueryLimitClause? LimitClause { get; set; }
    public QueryOffsetClause? OffsetClause { get; set; }
    private Table<T> _table;

    public QueryRoot(Table<T> table)
    {
        _table = table;
    }

    public override string Parse(CommandGenerator generator)
    {
        return generator.Select(_table, this);
    }
}