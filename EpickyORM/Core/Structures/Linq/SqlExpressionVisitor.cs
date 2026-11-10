using System.Linq.Expressions;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Linq.QueryModel;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq;

// projdu LINQ strom a namapuju si ho na svůj Query strom
// hodně mi pomohl tenhle blog: https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
public class SqlExpressionVisitor : ExpressionVisitor
{
    private int _currentParamId;
    private readonly QueryRoot _root;

    public SqlExpressionVisitor(Table table)
    {
        _root = new QueryRoot(table);
    }

    public QueryRoot Build(Expression expression)
    {
        Visit(expression);
        return _root;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        Visit(node.Arguments[0]); // rekurzivně jdu hledat další metody
        var content = node.Arguments[1];
        switch (node.Method.Name)
        {
            case "Where":
                _root.WhereClause =
                    new QueryWhereClause((QueryBinaryCondition)ParseExpression(content)); // Where(objects, x => ...)
                break;
            case "Take":
                _root.LimitClause = new QueryLimitClause((QueryParameter)ParseExpression(content)); // Take(objects, n)
                break;
            case "Skip":
                _root.OffsetClause = new QueryOffsetClause((QueryParameter)ParseExpression(content)); // Skip(objects, n)
                break;
            case "Distinct": // Select vždy vrací celý řádek, takže Distinct vlastně nedělá nic (jen brzdí databázi)
                _root.DistinctClause = new QueryDistinctClause(); // Distinct(objects)
                break;
            case "OrderBy":
                _root.OrderByClause = new QueryOrderByClause((QueryAttributeNames)ParseExpression(content), OrderByDirection.ASCENDING);
                break;
            case "OrderByDescending":
                _root.OrderByClause = new QueryOrderByClause((QueryAttributeNames)ParseExpression(content), OrderByDirection.DESCENDING);
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
            NewExpression nexp => new QueryAttributeNames(nexp.Arguments
                .Select(x => (QueryAttributeName)ParseExpression(x)).ToList()), // (x.Atr1, x.Atr2) -> Atr1, Atr2
            ConstantExpression param => new QueryParameter(param.Value,
                _currentParamId++), // díky zápočtovýmu testu jsem se naučil správně používat increment
            _ => throw new Exception("Unsupported expression")
        };
    }
}