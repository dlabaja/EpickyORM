using System.Linq.Expressions;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Structures.Linq;

// projdu LINQ strom a namapuju si ho na svůj Query strom
// hodně mi pomohl tenhle blog: https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
public class SqlExpressionVisitor<T> : ExpressionVisitor
{
    private readonly QueryRoot<T> _root;

    public SqlExpressionVisitor(Table<T> table)
    {
        _root = new QueryRoot<T>(table);
    }

    // poskládá strom od poslední LINQ metody k první (LIFO)
    public QueryRoot<T> Build(Expression expression)
    {
        Visit(expression);
        return _root;
    }

    // prochází všechny nody co jsou volání metody
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        Visit(node.Arguments[0]); // rekurzivně jdu hledat další metody

        // Count a podobné mají jen jeden argument = source
        // hodí se do budoucna kdybych ho chtěl implemetovat
        if (node.Arguments.Count == 1)
        {
            return node;
        }
        
        var content = node.Arguments[1];
        switch (node.Method.Name)
        {
            case "Where":
                _root.WhereClause =
                    new QueryWhereClause((QueryBinaryCondition)ParseExpression(content)); // Where(objects, x => ...)
                break;
            case "Take":
                _root.LimitClause = new QueryLimitClause((QueryParameter)ParseExpression(content), _root.Keeper); // Take(objects, n)
                break;
            case "Skip":
                _root.OffsetClause =
                    new QueryOffsetClause((QueryParameter)ParseExpression(content), _root.Keeper); // Skip(objects, n)
                break;
            case "Distinct": // Select vždy vrací celý řádek, takže Distinct vlastně nedělá nic (jen brzdí databázi)
                _root.DistinctClause = new QueryDistinctClause(); // Distinct(objects)
                break;
            case "OrderBy":
                _root.OrderByClause = QueryOrderByClause(ParseExpression(content), OrderByDirection.ASCENDING); // OrderBy(x => new{x.Id}) / OrderBy(x => x.Id)
                break;
            case "OrderByDescending":
                _root.OrderByClause = QueryOrderByClause(ParseExpression(content), OrderByDirection.DESCENDING); // OrderByDescending ...
                break;
            default:
                throw new Exception($"I don't know how to convert {node.Method.Name} to SQL yet ;(");
        }

        return node;
    }

    //    x => x.Id == 1
    // -> Id == 1
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
            ConstantExpression param => ConstantExpr(param), // 1, "ahoj", ...
            UnaryExpression unexp => ParseExpression(unexp.Operand), // {x => ...} je unary, s tím jsem moc nepočítal 
            _ => throw new Exception("Unsupported expression")
        };
    }
    
    private QueryParameter ConstantExpr(ConstantExpression param)
    {
        var res = new QueryParameter(param.Value, _root.Keeper);
        return res;
    }

    private QueryOrderByClause QueryOrderByClause(QueryExpression expression, OrderByDirection direction)
    {
        return expression switch
        {
            QueryAttributeNames names => new QueryOrderByClause(names, direction),
            QueryAttributeName name => new QueryOrderByClause(new QueryAttributeNames([name]), direction),
            _ => throw new Exception("Damn you broke the Order LINQ :/")
        };
    }
}