using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures;

namespace EpickyORMTest;

public class TestDriver : DatabaseDriver
{
    public TestDriver(string connectionString) : base(connectionString)
    {
    }

    protected override void Execute(string sql)
    {
    }

    public override void TryCreateTable(Table table)
    {
    }

    public override void DropTable(Table table)
    {
    }

    public override void Insert(Table table, List<Record> records)
    {
    }
}