using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers;

public abstract class DatabaseDriver
{
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

}