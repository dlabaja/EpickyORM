using EpickyORM.Core.Drivers;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures.Frontend;

namespace EpickyORMTest.Core;

public class DbTest<T> where T : DbContext
{
    private string? _path;
    private DatabaseDriver driver;
    protected Database<T> db;
    
    [SetUp]
    public void Setup()
    {
        // bez těch optionů mi to blokoval sql driver a nešlo nic přepsat
        _path = $"test_{TestContext.CurrentContext.Test.ID}.sqlite";
        if (!string.IsNullOrEmpty(_path))
        {
            File.Delete(_path);
        }
        driver = new SqliteDatabaseDriver($"Data Source={_path}");
        db = new Database<T>(driver);
    }

    [TearDown]
    public void TearDown()
    {
        if (!string.IsNullOrEmpty(_path))
        {
            File.Delete(_path);
        }
    }

    protected long GetCount(string tableName)
    {
        var res = driver.ExecuteScalar([$"SELECT COUNT(*) FROM {tableName}"]);
        return (long)res[0]!;
    }
}