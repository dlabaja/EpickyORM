using EpickyORM.Core.Attributes;
using EpickyORM.Core.Structures.Frontend;

namespace EpickyORMTest.Core;

public class RelationContext : DbContext
{
    public DbSet<Teacher2> Teachers { get; }
    public DbSet<RandomClass> RandomClasses { get; }
}

public class Teacher2
{
    [PrimaryKey] public int Id { get; set; } 
    public string Name { get; set; }
    public string Surname { get; set; }
    public RandomClass RandomClass { get; set; }
}

public class RandomClass
{
    [PrimaryKey] public int Id { get; set; } 
    public string Name { get; set; }
}

public class RelationsTest : DbTest<RelationContext>
{
    [Test]
    public void TestRelationSave()
    {
        var randomClass = new RandomClass { Id = 0, Name = "test" };
        var teacher = new Teacher2 {Id = 0, Name = "Jan", Surname = "Novák", RandomClass = randomClass};
        db.Context.Teachers.Add(teacher);
        db.Context.SaveChanges();
    }
    
    [Test]
    public void TestRelationSelect()
    {
        var randomClass = new RandomClass { Id = 0, Name = "test" };
        var teacher = new Teacher2 {Id = 0, Name = "Jan", Surname = "Novák", RandomClass = randomClass};
        db.Context.Teachers.Add(teacher);
        // relace to ukládá automaticky taky
        
        randomClass.Name = "test2";
        db.Context.SaveChanges();

        var selected = db.Context.Teachers.Where(x => x.Name == "Jan").ToList().First();
        Assert.That(selected.RandomClass.Name, Is.EqualTo("test2"));
    }
}