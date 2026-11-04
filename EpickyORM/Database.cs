namespace EpickyORM;

public class Database : IDisposable
{
    private string _connectionString;
    
    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    // todo získat jména tabulek z databáze?
    // možná ne, za runtimu stejně nepohlídám kontrolu
    public Table<T>? GetTable<T>(string tableName)
    {
        return new Table<T>();
    }

    public Table<T> CreateTable<T>(string tableName)
    {
        return new Table<T>();
    }

    public bool DeleteTable<T>(string tableName)
    {
        return true;
    }
    
    public string[] TableNames { get; }
    
    public void Dispose()
    {
        // todo zavřít databázi
    }
}