using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Frontend;

public class Database<T> where T : DbContext
{
    public T Context { get; }

    public Database(DatabaseDriver driver)
    {
        // tvorba kontextu
        Context = Activator.CreateInstance<T>();
        Context.Init(driver);
    }
}