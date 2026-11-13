using System.Collections;
using System.Linq.Expressions;

namespace EpickyORM.Core.Structures.Linq;

// https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
// třída kterou vrací každý LINQ příkaz
// musí to být Ordered, jinak by nefungovalo OrderBy
public class SqlQueryable<T> : IOrderedQueryable<T>
{
    // Expression = LINQ strom kterej se musí projít a překonvertovat na něco useful
    // proč tohle nemá modifikátor přístupu?
    // public -> při manipulaci s ORM by uživatel viděl Expression, Provider a ElementType
    // private/protected -> nemohl bych dědit z IQueryable
    // řešení? tahle věc které se říká Explicitní rozhraní
    // z nějakýho důvodu to jde stejně vidět při víc jak jednom dotazu (.Where().Expression)
    Expression IQueryable.Expression => _expression;
    // to co buildí a zpracovává expression
    IQueryProvider IQueryable.Provider => _provider;
    Type IQueryable.ElementType => typeof(T);
    
    private readonly IQueryProvider _provider;
    private readonly Expression _expression;

    // řetězení LINQ, předává přechozí expression
    public SqlQueryable(IQueryProvider provider, Expression expression)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _expression = expression;
    }
    
    // začátek LINQ, nastaví kořen na sebe
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