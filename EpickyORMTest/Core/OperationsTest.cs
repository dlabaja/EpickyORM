using EpickyORMTest.Mocks;

namespace EpickyORMTest.Core;

public class OperationsTest : DbTest
{
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
    
    [Test]
    public void TestAddToTableWithPk()
    {
        var teacher = new Teacher { Id = 20, Name = "Jan", Surname = "Novák", Titul = "Doc" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();
        Assert.That(teacher.Id, Is.EqualTo(20));
        Assert.That(GetCount("Teacher"), Is.EqualTo(1));
    }

    [Test]
    public void TestRemoveFromTable()
    {
        var teacher = new Teacher { Name = "Jan", Surname = "Novák", Titul = "Doc" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();
        db.Context.Teachers.Remove(teacher);
        db.Context.SaveChanges();
        Assert.That(GetCount("Teacher"), Is.EqualTo(0));
    }

    [Test]
    public void TestRemoveNotInTable()
    {
        var teacher = new Teacher { Name = "Jan", Surname = "Novák", Titul = "Doc" };
        db.Context.Teachers.Remove(teacher);
        db.Context.SaveChanges();
        Assert.That(GetCount("Teacher"), Is.EqualTo(0));
    }

    [Test]
    public void TestEditTwice()
    {
        var teacher = new Teacher { Name = "Jan", Surname = "Novák", Titul = "Doc" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();
        teacher.Name = "Adolf";
        db.Context.SaveChanges();
        var res = db.Context.Teachers.Where(x => x.Name == "Adolf");
        Assert.That(res.Count(), Is.EqualTo(1));
    }
}