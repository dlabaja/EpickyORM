using EpickyORM.Core.Attributes;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures;

class MyContext : DbContext
{
    public DbSet<Predmet> Predmets { get; set; } = new DbSet<Predmet>();
    public DbSet<Teacher> Teachers { get; set; } = new DbSet<Teacher>();
}

class Predmet
{
    public string Code { get; set; }
    public Teacher Teacher { get; set; }
    [Ignore] public string NotIncludedProp { get; set; }
    [Include] private string _includedField;
    private string _ignoredAutomatically;
    public string _ignoredAsWell;
}

class Teacher
{
    public string name;
    public string surname;
    public string titul;
}

public class Program
{
    public static void Main()
    {
        var db = new Database<MyContext>(new SqliteDatabaseDriver(""));
        
    }
}