using EpickyORM.Core.Drivers;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures.Frontend;
using EpickyORMTest.Mocks;

namespace EpickyORMTest.Core;

class MyContext : DbContext
{
    public DbSet<Teacher> Teachers { get; }
}

public class OperationsTest
{
    private readonly string _path = "test.sqlite";
    private static readonly DatabaseDriver driver = new SqliteDatabaseDriver("Data Source=test.sqlite");
    private static readonly Database<MyContext> db = new(driver);
    
    [SetUp]
    public void Setup()
    {
        File.WriteAllBytes(_path, []);
    }

    private static long GetCount(string tableName)
    {
        var res = driver.ExecuteScalar([$"SELECT COUNT(*) FROM {tableName}"]);
        return (long)res[0]!;
    }

    [Test]
    public void TestCreateTable()
    {
        // tabulka by měla být vytvořená automaticky při incializaci db (nahoře)
        Assert.That(GetCount("Teacher") == 0);
    }

    [Test]
    public void TestAddToTable()
    {
        var teacher = new Teacher { Name = "Jan", Surname = "Novák", Titul = "Doc" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();
        Assert.That(teacher.Id == 1);
        Assert.That(GetCount("Teacher") == 1);
    }
}