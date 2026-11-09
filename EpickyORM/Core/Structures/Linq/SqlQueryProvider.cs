using System.Linq.Expressions;

namespace EpickyORM.Core.Structures.Linq;

// překládá strom výrazů na useful sql
public class SqlQueryProvider : IQueryProvider
{
    // umožňuje LINQ chaining -> zabalí výraz do nové IQueryable
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
    }
    
    // projde strom výrazů a vrátí výsledek
    public TResult Execute<TResult>(Expression expression)
    {
        
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