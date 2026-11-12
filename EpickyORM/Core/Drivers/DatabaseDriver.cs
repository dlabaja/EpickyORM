using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;
using Microsoft.Data.Sqlite;

namespace EpickyORM.Core.Drivers;

public abstract class DatabaseDriver
{
    public abstract CommandGenerator CommandGenerator { get; }
    
    protected readonly string ConnectionString;

    protected DatabaseDriver(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    protected abstract T ExecuteInTransaction<T>(Func<SqliteConnection, SqliteTransaction, T> action);

    // je to public hlavně kvůli testům, ale i tak se to může někdy hodit
    public abstract void ExecuteVoid(string[] commands, ParameterKeeper? keeper = null);

    public abstract List<object?> ExecuteScalar(string[] commands, ParameterKeeper? keeper = null);
    
    public abstract List<Record<T>> ExecuteReader<T>(string[] commands, Table<T> table, ParameterKeeper? keeper = null);
    
    public abstract void TryCreateTable<T>(Table<T> table);
    
    // možná se někdy bude hodit (spíš ne)
    public abstract void DropTable<T>(Table<T> table);

    public abstract void Insert<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper, out int newId);

    public abstract List<Record<T>> Select<T>(Table<T> table, QueryRoot<T> root);

    // pokud někdo vytvoří entitu a upraví si sám PK, pak by update zhučel protože by takové PK neexistovalo. UPSERT to jistí 
    public abstract void Upsert<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper);

    public abstract void Delete<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper);
}