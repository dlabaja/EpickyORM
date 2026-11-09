using System.Collections;
using System.Linq.Expressions;

namespace EpickyORM.Core.Structures.Linq;

// https://jordansrowles.medium.com/building-a-custom-linq-provider-in-net-a987dc983381
// třída kterou vrací každý LINQ příkaz
public class SqlQueryable<T> : IQueryable<T>
{
    public SqlQueryable(IEnumerable<T> source)
    {
        
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}