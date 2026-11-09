using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures;
using EpickyORMTest.Mocks;

namespace EpickyORMTest.Core.Drivers.Sqlite;

public class SqliteTableConverterTest
{
    private readonly SqliteTableConverter _converter = new SqliteTableConverter();
    
    [Test]
    public void TestConvertType()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_converter.ConvertType(TypeCode.Boolean) == "INTEGER");
            Assert.That(_converter.ConvertType(TypeCode.Int16) == "INTEGER");
            Assert.That(_converter.ConvertType(TypeCode.Int32) == "INTEGER");
            Assert.That(_converter.ConvertType(TypeCode.Int64) == "INTEGER");
            Assert.That(_converter.ConvertType(TypeCode.Single) == "REAL");
            Assert.That(_converter.ConvertType(TypeCode.Double) == "REAL");
            Assert.That(_converter.ConvertType(TypeCode.String) == "TEXT");
            Assert.That(_converter.ConvertType(TypeCode.Char) == "TEXT");
            Assert.That(_converter.ConvertType(TypeCode.Empty) == "NULL");
            Assert.That(() => _converter.ConvertType(TypeCode.Object), Throws.Exception);
        }
    }
    
    [Test]
    public void TestConvertTableAttribute()
    {
        Assert.That(_converter.ConvertTableAttribute(new SchemaAttribute("id", typeof(int), true)) ==
                    "id INTEGER PRIMARY KEY");
        Assert.That(_converter.ConvertTableAttribute(new SchemaAttribute("id", typeof(string), true)) ==
                    "id TEXT PRIMARY KEY");
        Assert.That(_converter.ConvertTableAttribute(new SchemaAttribute("sus", typeof(int), false)) ==
                    "sus INTEGER");
    }

    [Test]
    public void TestConvertTable()
    {
        var table = new Table(typeof(Teacher));
        Assert.That(_converter.ConvertTable(table) ==
                    "Teacher (id INTEGER PRIMARY KEY, name TEXT, surname TEXT, titul TEXT)");
    }
    
    [Test]
    public void TestGetSchemaArray()
    {
        var table = new Table(typeof(Teacher));
        Assert.That(_converter.GetSchemaArray(table) ==
                    "(id, name, surname, titul)");
    }
    
    [Test]
    public void TestGetValuesArray()
    {
        var table = new Table(typeof(Teacher));
        var obj = new Teacher { Name = "Jan", Surname = "Novák", Titul = "doc" };
        var record = new Record(obj, typeof(Teacher), RecordAction.ADD);
        Assert.That(_converter.GetValuesArray(table, record) ==
                    """(0, "Jan", "Novák", "doc")""");
    }
    
    [Test]
    public void TestGetMultipleValuesArray()
    {
        var table = new Table(typeof(Teacher));
        var obj1 = new Teacher { Name = "Jan", Surname = "Novák", Titul = "doc" };
        var obj2 = new Teacher { Name = "Jana", Surname = "Nováková", Titul = "prof" };
        var record1 = new Record(obj1, typeof(Teacher), RecordAction.ADD);
        var record2 = new Record(obj2, typeof(Teacher), RecordAction.ADD);
        Assert.That(_converter.GetMultipleValuesArray(table, [record1, record2]) ==
                    """(0, "Jan", "Novák", "doc"), (0, "Jana", "Nováková", "prof")""");
    }
    
    [Test]
    public void TestFormatValue()
    {
        Assert.That(_converter.FormatValue("ahoj") == "\"ahoj\"");
        Assert.That(_converter.FormatValue(1) == "1");
        Assert.That(_converter.FormatValue(1.2), Is.EqualTo("1.2"));
    }
}