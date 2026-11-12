namespace EpickyORMTest.Core;

public class LinqTest : DbTest<MyContext>
{
    [Test]
    public void TestWhere()
    {
        var teacher = new Teacher { Id = 0, Name = "Jan", Surname = "Novák" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();

        // ToList enumeruje můj custom LINQ, až potom můžu bezpečně používat klasický LINQ metody (už jsou nad kolekcí, ne nad generátorem SQL)
        var selected = db.Context.Teachers.Where(x => x.Name == "Jan").ToList().First();
        Assert.That(teacher, Is.EqualTo(selected));
    }
    
    [Test]
    public void TestLimit()
    {
        var teacher1 = new Teacher { Id = 0, Name = "Jan", Surname = "Novák" };
        var teacher2 = new Teacher { Id = 0, Name = "Jana", Surname = "Novák" };
        var teacher3 = new Teacher { Id = 0, Name = "Janaa", Surname = "Novák" };
        var teacher4 = new Teacher { Id = 0, Name = "Janaaa", Surname = "Novák" };
        db.Context.Teachers.Add(teacher1);
        db.Context.Teachers.Add(teacher2);
        db.Context.Teachers.Add(teacher3);
        db.Context.Teachers.Add(teacher4);
        db.Context.SaveChanges();

        for (var i = 0; i <= 5; i++)
        {
            var count = db.Context.Teachers.Take(i).ToList().Count;
            Assert.That(count, Is.EqualTo(Math.Clamp(i, 0, 4)));
        }
    }
    
    [Test]
    public void TestOffset()
    {
        var teacher1 = new Teacher { Id = 0, Name = "Jan", Surname = "Novák" };
        var teacher2 = new Teacher { Id = 0, Name = "Jana", Surname = "Novák" };
        var teacher3 = new Teacher { Id = 0, Name = "Janaa", Surname = "Novák" };
        var teacher4 = new Teacher { Id = 0, Name = "Janaaa", Surname = "Novák" };
        db.Context.Teachers.Add(teacher1);
        db.Context.Teachers.Add(teacher2);
        db.Context.Teachers.Add(teacher3);
        db.Context.Teachers.Add(teacher4);
        db.Context.SaveChanges();
        
        Assert.That(db.Context.Teachers.Skip(0).ToList().First(), Is.EqualTo(teacher1));
        Assert.That(db.Context.Teachers.Skip(1).ToList().First(), Is.EqualTo(teacher2));
        Assert.That(db.Context.Teachers.Skip(2).ToList().First(), Is.EqualTo(teacher3));
        Assert.That(db.Context.Teachers.Skip(3).ToList().First(), Is.EqualTo(teacher4));
        Assert.That(db.Context.Teachers.Skip(4).ToList().FirstOrDefault(), Is.Null);
    }
    
    [Test]
    public void TestDistinct()
    {
        var teacher = new Teacher { Id = 0, Name = "Jan", Surname = "Novák" };
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();

        // distinct by nemělo udělat nic (vždy volám SELECT *)
        // v postgre je něco jako DistinctBy, tam už by to něco dělat mohlo
        var selected = db.Context.Teachers.Where(x => x.Name == "Jan").ToList().First();
        var distinct = db.Context.Teachers.Where(x => x.Name == "Jan").Distinct().ToList().First();
        Assert.That(selected, Is.EqualTo(distinct));
    }
    
    [Test]
    public void TestOrderBy()
    {
        var teacher1 = new Teacher { Id = 1, Name = "A", Surname = "Novák" };
        var teacher4 = new Teacher { Id = 4, Name = "B", Surname = "Novák" };
        var teacher2 = new Teacher { Id = 2, Name = "C", Surname = "Novák" };
        var teacher3 = new Teacher { Id = 3, Name = "D", Surname = "Novák" };
        db.Context.Teachers.Add(teacher1);
        db.Context.Teachers.Add(teacher2);
        db.Context.Teachers.Add(teacher3);
        db.Context.Teachers.Add(teacher4);
        db.Context.SaveChanges();

        // single argument
        var orderedAsc = db.Context.Teachers.OrderBy(x => x.Id).ToList();
        Assert.That(orderedAsc[0].Id, Is.EqualTo(1));
        Assert.That(orderedAsc[1].Id, Is.EqualTo(2));
        Assert.That(orderedAsc[2].Id, Is.EqualTo(3));
        Assert.That(orderedAsc[3].Id, Is.EqualTo(4));
        
        var orderedDesc = db.Context.Teachers.OrderByDescending(x => x.Id).ToList();
        Assert.That(orderedDesc[0].Id, Is.EqualTo(4));
        Assert.That(orderedDesc[1].Id, Is.EqualTo(3));
        Assert.That(orderedDesc[2].Id, Is.EqualTo(2));
        Assert.That(orderedDesc[3].Id, Is.EqualTo(1));
        
        // více argumentů
        orderedAsc = db.Context.Teachers.OrderBy(x => new {x.Id, x.Name}).ToList();
        Assert.That(orderedAsc[0].Id, Is.EqualTo(1));
        Assert.That(orderedAsc[1].Id, Is.EqualTo(2));
        Assert.That(orderedAsc[2].Id, Is.EqualTo(3));
        Assert.That(orderedAsc[3].Id, Is.EqualTo(4));
        
        orderedDesc = db.Context.Teachers.OrderByDescending(x => new {x.Id, x.Name}).ToList();
        Assert.That(orderedDesc[0].Id, Is.EqualTo(4));
        Assert.That(orderedDesc[1].Id, Is.EqualTo(3));
        Assert.That(orderedDesc[2].Id, Is.EqualTo(2));
        Assert.That(orderedDesc[3].Id, Is.EqualTo(1));
    }
}