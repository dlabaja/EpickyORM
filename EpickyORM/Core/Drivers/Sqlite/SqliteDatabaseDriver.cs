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
    protected override void Execute(string[] commands)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        foreach (var command in commands)
        {
            using var cmd = new SqliteCommand(command, connection);
            cmd.ExecuteNonQuery();
        }
    }
    
    protected override List<object?> ExecuteScalar(string[] commands)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        return commands.Select(command => new SqliteCommand(command, connection).ExecuteScalar()).ToList();
    }

    public override void TryCreateTable(Table table)
    {
        Execute([_generator.TryCreateTable(table)]);
    }

    public override void DropTable(Table table)
    {
        Execute([_generator.DropTable(table)]);
    }

    public override void Insert(Table table, List<Record> records, out int lastNewId)
    {
        lastNewId = 0;
        var objects = ExecuteScalar([_generator.Insert(table, records), _generator.GetLastId()]);
        int.TryParse(objects.Last()?.ToString(), out lastNewId);
    }
}