using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Clause;

public class QueryWhereClause : QueryExpression
{
    public QueryBinaryCondition BinaryCondition { get; }
    
    public QueryWhereClause(QueryBinaryCondition binaryCondition)
    {
        BinaryCondition = binaryCondition;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetWhereClause(this);
    }
}