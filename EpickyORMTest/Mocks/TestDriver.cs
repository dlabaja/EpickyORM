using EpickyORM.Core.Drivers;
using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;

namespace EpickyORMTest.Mocks;

public class TestDriver : DatabaseDriver
{
    public TestDriver(string connectionString) : base(connectionString)
    {
    }

    public override CommandGenerator CommandGenerator { get; }
    public override void Execute(string[] commands)
    {
        
    }

    public override List<object?> ExecuteScalar(string[] commands)
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

    public override List<Record> Select(Table table, QueryRoot root)
    {
        throw new NotImplementedException();
    }

    public override void Update(Table table, List<Record> records)
    {
        throw new NotImplementedException();
    }

    public override void Delete(Table table, List<Record> records)
    {
        throw new NotImplementedException();
    }
}