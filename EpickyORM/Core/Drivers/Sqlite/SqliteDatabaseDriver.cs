using Microsoft.Data.Sqlite;

namespace EpickyORM.Core.Drivers.SQLite;

public class SqliteDatabaseDriver : DatabaseDriver
{
    private readonly SqliteConnection _connection;
    
    public SqliteDatabaseDriver(string connectionString) : base(connectionString)
    {
        _connection = new SqliteConnection(ConnectionString);
    }

    public override void Connect()
    {
        _connection.Open();
    }

    public override void Close()
    {
        _connection.Close();
    }

    public override void CreateTable(string name)
    {
        throw new NotImplementedException();
    }

    public override void DropTable(string name)
    {
        throw new NotImplementedException();
    }
}