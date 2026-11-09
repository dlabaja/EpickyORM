using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures;

namespace EpickyORMTest.Mocks;

public class TestDriver : DatabaseDriver
{
    public TestDriver(string connectionString) : base(connectionString)
    {
    }

    protected override void Execute(string[] commands)
    {
        
    }

    protected override List<object?> ExecuteScalar(string[] commands)
    {
        return [];
    }

    public override void TryCreateTable(Table table)
    {
    }

    public override void DropTable(Table table)
    {
    }

    public override void Insert(Table table, List<Record> records, out int newId)
    {
        newId = 0;
    }
}