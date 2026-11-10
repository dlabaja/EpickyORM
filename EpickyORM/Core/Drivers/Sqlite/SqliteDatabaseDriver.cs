using EpickyORM.Core.Drivers.Sqlite;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;
using Microsoft.Data.Sqlite;
using static System.Int32;

namespace EpickyORM.Core.Drivers.SQLite;

public class SqliteDatabaseDriver : DatabaseDriver
{
    private readonly CommandGenerator _generator = new SqliteCommandGenerator();
    public override CommandGenerator CommandGenerator => _generator;

    public SqliteDatabaseDriver(string connectionString) : base(connectionString)
    {
    }

    // https://www.sqlitetutorial.net/sqlite-csharp/create-tables/
    // connection by se měl automaticky poolovat, takže můžu udělat jeden na každej příkaz
    // transakce kromě možnosti rollbacku umožňuje nešahat kvůli každému příkazu na disk
    protected override void Execute(string[] commands)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        foreach (var command in commands)
        {
            using var cmd = new SqliteCommand(command, connection);
            cmd.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    protected override List<object?> ExecuteScalar(string[] commands)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        var result = commands.Select(command => new SqliteCommand(command, connection).ExecuteScalar()).ToList();
        transaction.Commit();
        return result;
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
        TryParse(objects.Last()?.ToString(), out lastNewId);
    }

    public override List<Record> Select(Table table, QueryRoot root)
    {
        var result = new List<Record>();
        var objects = ExecuteScalar([_generator.Select(table, root)]);
        foreach (var obj in objects)
        {
            if (obj == null)
            {
                continue;
            }

            result.Add(new Record(obj, table.Type, RecordAction.NONE));
        }

        return result;
    }

    public override void Update(Table table, List<Record> records)
    {
        Execute([
            .. records.Select(x => _generator.Update(table, x, table.GetRecordPk(x)))
        ]);
    }

    public override void Delete(Table table, List<Record> records)
    {
        Execute([
            .. records.Select(x => _generator.Delete(table, table.GetRecordPk(x)))
        ]);
    }
}