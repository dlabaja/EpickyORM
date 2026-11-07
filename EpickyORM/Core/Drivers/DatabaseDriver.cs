namespace EpickyORM.Core.Drivers;

public abstract class DatabaseDriver : IDisposable
{
    protected readonly string ConnectionString;

    protected DatabaseDriver(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public abstract void Connect();

    public abstract void Close();
    
    public abstract void CreateTable(string name);
    
    public abstract void DropTable(string name);
    
    

    public void Dispose()
    {
        Close();
    }
}