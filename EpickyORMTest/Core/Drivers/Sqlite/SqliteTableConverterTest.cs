using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;
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
            Assert.That(_converter.ConvertType(TypeCode.Boolean), Is.EqualTo("INTEGER"));
            Assert.That(_converter.ConvertType(TypeCode.Int16), Is.EqualTo("INTEGER"));
            Assert.That(_converter.ConvertType(TypeCode.Int32), Is.EqualTo("INTEGER"));
            Assert.That(_converter.ConvertType(TypeCode.Int64), Is.EqualTo("INTEGER"));
            Assert.That(_converter.ConvertType(TypeCode.Single), Is.EqualTo("REAL"));
            Assert.That(_converter.ConvertType(TypeCode.Double), Is.EqualTo("REAL"));
            Assert.That(_converter.ConvertType(TypeCode.String), Is.EqualTo("TEXT"));
            Assert.That(_converter.ConvertType(TypeCode.Char), Is.EqualTo("TEXT"));
            Assert.That(_converter.ConvertType(TypeCode.Empty), Is.EqualTo("NULL"));
            Assert.That(() => _converter.ConvertType(TypeCode.Object), Throws.Exception);
        }
    }
    
    [Test]
    public void TestConvertTableAttribute()
    {
        Assert.That(_converter.ConvertTableAttribute(new SchemaAttribute("id", typeof(int), true, false)), Is.EqualTo("id INTEGER PRIMARY KEY"));
        Assert.That(_converter.ConvertTableAttribute(new SchemaAttribute("id", typeof(string), true, false)), Is.EqualTo("id TEXT PRIMARY KEY"));
        Assert.That(_converter.ConvertTableAttribute(new SchemaAttribute("sus", typeof(int), false, false)), Is.EqualTo("sus INTEGER"));
    }

    [Test]
    public void TestConvertTable()
    {
        var table = new Table<Teacher>();
        Assert.That(_converter.ConvertTable(table), Is.EqualTo("Teacher (id INTEGER PRIMARY KEY, name TEXT, surname TEXT, titul TEXT)"));
    }
    
    [Test]
    public void TestGetSchemaArray()
    {
        var table = new Table<Teacher>();
        Assert.That(_converter.GetSchemaArray(table, false), Is.EqualTo("(id, name, surname, titul)"));
    }
    
    [Test]
    public void TestGetValuesArray()
    {
        var table = new Table<Teacher>();
        var obj = new Teacher { Name = "Jan", Surname = "Novák", Titul = "doc" };
        var record = new Record<Teacher>(obj, RecordAction.ADD);
        Assert.That(_converter.GetValuesArray(table, record, new ParameterKeeper(), false), Is.EqualTo("(@p1, @p2, @p3, @p4)"));
    }
    
    [Test]
    public void TestGetMultipleValuesArray()
    {
        var table = new Table<Teacher>();
        var obj1 = new Teacher { Name = "Jan", Surname = "Novák", Titul = "doc" };
        var obj2 = new Teacher { Name = "Jana", Surname = "Nováková", Titul = "prof" };
        var record1 = new Record<Teacher>(obj1, RecordAction.ADD);
        var record2 = new Record<Teacher>(obj2, RecordAction.ADD);
        Assert.That(_converter.GetMultipleValuesArray(table, [record1, record2], new ParameterKeeper(), false), 
            Is.EqualTo("(@p1, @p2, @p3, @p4), (@p1, @p5, @p6, @p7)"));
    }
    
    [Test]
    public void TestFormatValue()
    {
        Assert.That(_converter.FormatValue("ahoj"), Is.EqualTo("\"ahoj\""));
        Assert.That(_converter.FormatValue(1), Is.EqualTo("1"));
        Assert.That(_converter.FormatValue(1.2), Is.EqualTo("1.2"));
    }
}