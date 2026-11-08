using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers.Sqlite;

public class SqliteCommandGenerator : CommandGenerator
{
    private readonly SqliteTableConverter _converter = new();
    
    public override string TryCreateTable(Table table)
    {
        return $"CREATE TABLE IF NOT EXISTS {_converter.ConvertTable(table)};";
    }

    public override string DropTable(Table table)
    {
        return $"DROP TABLE {table.Name};";
    }

    public override string Insert(Table table, List<Record> records)
    {
        return
            $"INSERT INTO {table.Name} {_converter.GetSchemaArray(table)} VALUES {_converter.GetMultipleValuesArray(table, records)};";
    }
}