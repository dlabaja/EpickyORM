using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers;

public abstract class DatabaseDriver
{
    protected readonly string ConnectionString;

    protected DatabaseDriver(string connectionString)
    {
        ConnectionString = connectionString;
    }

    protected abstract void Execute(string sql);
    
    public abstract void CreateTable(Table table);
    
    public abstract void DropTable(Table table);

    public abstract void AddToTable(Table table);
}