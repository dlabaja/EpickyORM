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
    
    // umožňuje řetězení
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new SqlQueryable<TElement>(this, expression);
    }
    
    // zavolá se po vyhodnocení LINQ - zatím vrací kolekci, kdybych implementoval Count tak může vracet i číslo
    public TResult Execute<TResult>(Expression expression)
    {
        var visitor = new SqlExpressionVisitor<T>(_table);
        var queryRoot = visitor.Build(expression);
        var result = _driver.Select(_table, queryRoot);
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