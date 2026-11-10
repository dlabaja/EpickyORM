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
    private static readonly DatabaseDriver _driver = new SqliteDatabaseDriver("Data Source=test.sqlite");
    private static readonly Database<MyContext> _db = new(_driver);
    
    [SetUp]
    public void Setup()
    {
        File.WriteAllBytes(_path, []);
    }

    [Test]
    public void TestCreateTable()
    {
        // tabulka by měla být vytvořená automaticky na začátku (nesmí to spadnout)
        var res = _driver.ExecuteScalar(["SELECT COUNT(*) FROM Teacher"]);
        Assert.That((long)res[0]! == 0);
    }

    [Test]
    public void TestAddToTable()
    {
        _db.Context.Teachers.Add(new Teacher() {Name = "Jan", Surname = "Novák", Titul = "Doc"});
    }
}