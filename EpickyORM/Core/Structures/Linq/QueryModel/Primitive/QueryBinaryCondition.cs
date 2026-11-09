using System.Linq.Expressions;
using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

public class QueryBinaryCondition : QueryExpression
{
    public QueryExpression Left { get; }
    public QueryExpression Right { get; }
    public ExpressionType BinaryOperator { get; }
    
    public QueryBinaryCondition(QueryExpression left, QueryExpression right, ExpressionType binaryOperator)
    {
        Left = left;
        Right = right;
        BinaryOperator = binaryOperator;
    }
    
    public override string Parse(CommandGenerator generator)
    {
        return generator.GetBinaryCondition(this);
    }
}