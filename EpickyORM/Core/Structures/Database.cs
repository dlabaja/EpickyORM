namespace EpickyORM.Core.Structures;

public class Database<D> : IDisposable where D : DbContext
{
    private string _connectionString;
    
    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect()
    {
        S
    }

    public string[] TableNames { get; }

    public void Dispose()
    {
        // todo zavřít databázi
    }
}