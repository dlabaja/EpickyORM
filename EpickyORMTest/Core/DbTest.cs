using EpickyORM.Core.Drivers;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures.Frontend;
using EpickyORMTest.Mocks;

namespace EpickyORMTest.Core;

public class DbTest
{
    private string? _path;
    private DatabaseDriver driver;
    protected Database<MyContext> db;
    
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
        db = new Database<MyContext>(driver);
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