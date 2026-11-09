using System.Linq.Expressions;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Enums;

namespace EpickyORM.Core.Structures.Linq;

// tree walker, tady vařím
// hodně mi pomohl tenhle blog: https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
public class SqlExpressionVisitor : ExpressionVisitor
{
    private List<Clause> _clauses = new();
    private CommandGenerator _generator;
    private Clause? _currentClause = null;

    private SqlExpressionVisitor(CommandGenerator generator)
    {
        _generator = generator;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case "Where":
                VisitWhere(node);
                break;
            case "OrderBy":
                VisitOrderBy(node);
                break;
            default:
                throw new Exception($"I don't know how to convert {node.Method.Name} to SQL ;(");
        }

        return node;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _currentClause?.AddBodyPart();
        Visit(node.Left);
        _generator.GetBinaryOperator(node.NodeType);
        Visit(node.Right);
        return node;
    }

    private void VisitWhere(MethodCallExpression node)
    {
        _currentClause = GetClause(ClauseOperator.WHERE);
        Visit(node.Arguments[1]); // podmínka
    }

    private void VisitOrderBy(MethodCallExpression node)
    {
        Visit(node.Arguments[1]); // taky podmínka
    }

    private Clause GetClause(ClauseOperator op)
    {
        var clause = _clauses.Find(x => x.Operator == op);
        if (clause == null)
        {
            var c = new Clause(op);
            _clauses.Add(c);
            return c;
        }
        return clause;
    }

    public string Build()
    {
        var result = new List<string>();
        foreach (var op in Enum.GetValues<ClauseOperator>())
        {
            var clause = _clauses.Find(x => x.Operator == op);
            if (clause == null)
            {
                continue;
            }

            result.Add(clause.Build(_generator));
        }

        return string.Join(" ", result);
    }
    
}