using System.Collections;
using System.Linq.Expressions;

namespace EpickyORM.Core.Structures.Linq;

// https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
// třída kterou vrací každý LINQ příkaz
public class SqlQueryable<T> : IQueryable<T>
{
    // proč tohle nemá modifikátor přístupu?
    // public -> při manipulaci s ORM by uživatel viděl Expression, Provider a ElementType
    // private/protected -> nemohl bych dědit z IQueryable
    // řešení? tahle věc které se říká Explicitní rozhraní
    Expression IQueryable.Expression => _expression;
    IQueryProvider IQueryable.Provider => _provider;
    Type IQueryable.ElementType => typeof(T);
    
    private readonly IQueryProvider _provider;
    private readonly Expression _expression;

    public SqlQueryable(IQueryProvider provider, Expression expression)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _expression = expression;
    }

    public SqlQueryable(IQueryProvider provider)
    {
        _provider = provider;
        _expression = Expression.Constant(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
    }
}