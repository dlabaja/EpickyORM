using System.Linq.Expressions;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Linq.QueryModel;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq;

// projdu LINQ strom a namapuju si ho na svůj Query strom
// hodně mi pomohl tenhle blog: https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
public class SqlExpressionVisitor : ExpressionVisitor
{
    private CommandGenerator _generator;
    private int _currentParamId;
    private QueryRoot _root = new QueryRoot();

    public SqlExpressionVisitor(CommandGenerator generator)
    {
        _generator = generator;
    }
    
    public QueryRoot Build(Expression expression)
    {
        Visit(expression);
        return _root;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        Visit(node.Arguments[0]); // rekurzivně jdu hledat další metody
        switch (node.Method.Name)
        {
            case "Where":
                _root.WhereClause = new QueryWhereClause((QueryBinaryCondition)ParseExpression(node.Arguments[1])); // Where(objects, x => ...)
                break;
            default:
                throw new Exception($"I don't know how to convert {node.Method.Name} to SQL ;(");
        }

        return node;
    }
    
    // x => x.Id == 1
    // Id == 1
    private QueryExpression ParseExpression(Expression expr)
    {
        return expr switch
        {
            BinaryExpression bin => new QueryBinaryCondition(ParseExpression(bin.Left), ParseExpression(bin.Right),
                bin.NodeType),
            MemberExpression mem => new QueryAttributeName(mem.Member.Name), // e.Arg -> "Arg"
            LambdaExpression lambda => ParseExpression(lambda.Body), // (e => e.Arg) -> e.Arg
            ConstantExpression param => new QueryParameter(param.Value,
                _currentParamId++), // díky zápočtovýmu testu jsem se naučil správně používat increment
            _ => throw new Exception("Unsupported expression")
        };
    }
}