using EpickyORM.Core.Drivers.Sqlite;
using EpickyORM.Core.Structures;
using Microsoft.Data.Sqlite;

namespace EpickyORM.Core.Drivers.SQLite;

public class SqliteDatabaseDriver : DatabaseDriver
{
    private CommandGenerator _generator = new SqliteCommandGenerator();
    
    public SqliteDatabaseDriver(string connectionString) : base(connectionString)
    {
    }
    
    // https://www.sqlitetutorial.net/sqlite-csharp/create-tables/
    // connection by se měl automaticky poolovat, takže můžu udělat jeden na každej příkaz
    protected override void Execute(string sql)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    public override void TryCreateTable(Table table)
    {
        Execute(_generator.TryCreateTable(table));
    }

    public override void DropTable(Table table)
    {
        Execute(_generator.DropTable(table));
    }

    public override void Insert(Table table, List<Record> records)
    {
        Execute(_generator.Insert(table, records));
    }
}