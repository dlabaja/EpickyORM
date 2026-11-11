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
    private DatabaseDriver driver;
    private Database<MyContext> db;
    
    [SetUp]
    public void Setup()
    {
        // bez těch optionů mi to blokoval sql driver a nešlo nic přepsat
        var file = File.Open(_path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        file.Write([]);
        driver = new SqliteDatabaseDriver($"Data Source={_path}");
        db = new Database<MyContext>(driver);
    }

    private long GetCount(string tableName)
    {
        var res = driver.ExecuteScalar([$"SELECT COUNT(*) FROM {tableName}"]);
        return (long)res[0]!;
    }

    [Test]
    public void TestCreateTable()
    {
        // tabulka by měla být vytvořená automaticky při incializaci db (v setupu)
        Assert.That(GetCount("Teacher"), Is.EqualTo(0));
    }

    [Test]
    public void TestAddToTable()
    {
        var teacher = new Teacher { Name = "Jan", Surname = "Novák", Titul = "Doc" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();
        Assert.That(teacher.Id, Is.EqualTo(1));
        Assert.That(GetCount("Teacher"), Is.EqualTo(1));
    }
}