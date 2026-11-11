using EpickyORM.Core.Attributes;
using EpickyORM.Core.Structures.Frontend;

namespace EpickyORMTest.Mocks;

public class MyContext : DbContext
{
    public DbSet<Teacher> Teachers { get; }
}

class Predmet
{
    [PrimaryKey] public int Id { get; set; } 
    public string Code { get; set; }
    public Teacher Teacher { get; set; }
    [EpickyORM.Core.Attributes.Ignore] public string NotIncludedProp { get; set; }
    [Include] private string _includedField;
    private string _ignoredAutomatically;
    public string _ignoredAsWell;
}

public class Teacher
{
    [PrimaryKey] public int Id { get; set; } 
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Titul { get; set; }
}