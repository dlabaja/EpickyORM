using System.Linq.Expressions;
using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;

namespace EpickyORM.Core.Structures.Linq;

// překládá strom výrazů na useful sql
public class SqlQueryProvider<T> : IQueryProvider
{
    private readonly Table<T> _table;
    private readonly DatabaseDriver _driver;

    public SqlQueryProvider(Table<T> table, DatabaseDriver driver)
    {
        _table = table;
        _driver = driver;
    }
    
    // umožňuje LINQ chaining -> zabalí výraz do nové IQueryable
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new SqlQueryable<TElement>(this, expression);
    }
    
    // zavolá se po vyhodnocení LINQ - TResult může být kolekce nebo číslo (Count)
    public TResult Execute<TResult>(Expression expression)
    {
        var visitor = new SqlExpressionVisitor<T>(_table);
        var queryRoot = visitor.Build(expression);
        var result = _driver.Select(_table, queryRoot);
        if (expression is MethodCallExpression method)
        {
            switch (method.Method.Name)
            {
                case "Count":
                    return (TResult)(object)result.Count;
                case "Any":
                    return (TResult)(object)(result.Count != 0);
                case "First":
                    return (TResult)(object)result.First();
                default:
                    throw new Exception("I didn't implement this LINQ yet, I hope you didn't run it on production...");
            }
        }
        
        return (TResult)result.Select(x => x.Instance);
    }

    // řeším jenom generiku
    public object? Execute(Expression expression)
    {
        throw new NotImplementedException();
    }
    
    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotImplementedException();
    }
}