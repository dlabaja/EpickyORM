using EpickyORM.Core.Attributes;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures;

class MyContext : DbContext
{
    public DbSet<Predmet> Predmets { get; }
    public DbSet<Teacher> Teachers { get; }
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