using EpickyORM.Core.Drivers.Sqlite;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;
using EpickyORMTest.Mocks;

namespace EpickyORMTest.Core.Drivers.Sqlite;

public class SqliteCommandGeneratorTest
{
    private SqliteCommandGenerator _generator = new SqliteCommandGenerator();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestCreateTable()
    {
        var table = new Table(typeof(Teacher));
        Assert.That(_generator.TryCreateTable(table), Is.EqualTo(
            "CREATE TABLE IF NOT EXISTS Teacher (id INTEGER PRIMARY KEY, name TEXT, surname TEXT, titul TEXT);"));
    }

    [Test]
    public void TestDropTable()
    {
        var table = new Table(typeof(Teacher));
        Assert.That(_generator.DropTable(table), Is.EqualTo(
            "DROP TABLE Teacher;"));
    }

    [Test]
    public void TestInsert()
    {
        var table = new Table(typeof(Teacher));
        
        var obj1 = new Teacher { Name = "Jan", Surname = "Novák", Titul = "doc" };
        var obj2 = new Teacher { Name = "Jana", Surname = "Nováková", Titul = "prof" };
        var record1 = new Record(obj1, typeof(Teacher), RecordAction.ADD);
        var record2 = new Record(obj2, typeof(Teacher), RecordAction.ADD);

        Console.WriteLine(_generator.Insert(table, [record1, record2]));
        Assert.That(_generator.Insert(table, [record1, record2]), Is.EqualTo(
            """INSERT INTO Teacher (id, name, surname, titul) VALUES (0, "Jan", "Novák", "doc"), (0, "Jana", "Nováková", "prof");"""));
    }
}