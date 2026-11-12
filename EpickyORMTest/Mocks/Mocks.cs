using EpickyORM.Core.Attributes;
using EpickyORM.Core.Structures.Frontend;

public class MyContext : DbContext
{
    public DbSet<Teacher> Teachers { get; }
}

public class Teacher
{
    [PrimaryKey] public int Id { get; set; } 
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Titul { get; set; }
}