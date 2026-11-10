using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;

namespace EpickyORM.Core.Drivers;

public abstract class DatabaseDriver
{
    public abstract CommandGenerator CommandGenerator { get; }
    
    protected readonly string ConnectionString;

    protected DatabaseDriver(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected abstract void Execute(string[] commands);

    protected abstract List<object?> ExecuteScalar(string[] commands);
    
    public abstract void TryCreateTable(Table table);
    
    public abstract void DropTable(Table table);

    public abstract void Insert(Table table, List<Record> records, out int newId);

    public abstract List<Record> Select(Table table, QueryRoot root);

    public abstract void Update(Table table, List<Record> records);

    public abstract void Delete(Table table, List<Record> records);
}