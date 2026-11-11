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

    // je to public hlavně kvůli testům, ale i tak se to může někdy hodit
    public abstract void Execute(string[] commands);

    public abstract List<object?> ExecuteScalar(string[] commands);
    
    public abstract void TryCreateTable<T>(Table<T> table);
    
    public abstract void DropTable<T>(Table<T> table);

    public abstract void Insert<T>(Table<T> table, List<Record<T>> records, out int newId);

    public abstract List<Record<T>> Select<T>(Table<T> table, QueryRoot<T> root);

    public abstract void Update<T>(Table<T> table, List<Record<T>> records);

    public abstract void Delete<T>(Table<T> table, List<Record<T>> records);
}