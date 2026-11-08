using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures;

public class Database<T> where T : DbContext
{
    private DatabaseDriver _driver;

    public Database(DatabaseDriver driver)
    {
        _driver = driver;
    }
}