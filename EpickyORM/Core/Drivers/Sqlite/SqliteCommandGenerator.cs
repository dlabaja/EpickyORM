using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers.Sqlite;

public class SqliteCommandGenerator : CommandGenerator
{
    private readonly SqliteTableConverter _converter = new SqliteTableConverter();
    
    public override string CreateTable(Table table)
    {
        return $"CREATE TABLE {_converter.ConvertTable(table)};";
    }

    public override string DropTable(Table table)
    {
        return $"DROP TABLE {table.Name};";
    }
}