using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures;

public class Database<T> where T : DbContext
{
    public T Context { get; }
    private readonly DatabaseDriver _driver;

    public Database(DatabaseDriver driver)
    {
        _driver = driver;
        Context = Activator.CreateInstance<T>();
        Context.Driver = driver;
    }
}