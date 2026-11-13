using EpickyORM.Core.Drivers.Sqlite;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;
using Microsoft.Data.Sqlite;
using static System.Int32;

namespace EpickyORM.Core.Drivers.SQLite;

public class SqliteDatabaseDriver : DatabaseDriver
{
    private readonly CommandGenerator _generator = new SqliteCommandGenerator();
    protected override CommandGenerator CommandGenerator => _generator;

    public SqliteDatabaseDriver(string connectionString) : base(connectionString)
    {
    }

    // https://www.sqlitetutorial.net/sqlite-csharp/create-tables/
    // connection by se měl automaticky poolovat, takže můžu udělat jeden na každej příkaz
    // transakce kromě možnosti rollbacku umožňuje nešahat kvůli každému příkazu na disk
    protected override T ExecuteInTransaction<T>(Func<SqliteConnection, SqliteTransaction, T> action)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            var result = action(connection, transaction);
            transaction.Commit();
            return result;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new Exception("Error in my ORM Execute, it's still Epicky though: " + e.Message);
        }
    }

    // nevrací nic
    public override void ExecuteVoid(string[] commands, ParameterKeeper? keeper = null)
    {
        ExecuteInTransaction((connection, transaction) =>
        {
            foreach (var command in commands)
            {
                using var cmd = new SqliteCommand(command, connection, transaction);
                if (keeper != null && keeper.Parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(keeper.Parameters
                        .Select(x =>
                            new SqliteParameter(_generator.GetParameter(x.Value, keeper),
                                _generator.GetValue(x.Value))));
                }

                cmd.ExecuteNonQuery();
            }

            return new { }; // placeholder, stejně se ignoruje
        });
    }

    // vrací čísla
    public override List<object?> ExecuteScalar(string[] commands, ParameterKeeper? keeper = null)
    {
        return ExecuteInTransaction((connection, transaction) =>
        {
            var result = new List<object?>();
            foreach (var command in commands)
            {
                using var cmd = new SqliteCommand(command, connection, transaction);
                if (keeper != null && keeper.Parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(keeper.Parameters
                        .Select(x =>
                            new SqliteParameter(_generator.GetParameter(x.Value, keeper),
                                _generator.GetValue(x.Value))));
                }

                result.Add(cmd.ExecuteScalar());
            }

            return result;
        });
    }

    // vrací množinu záznamů
    public override List<Record<T>> ExecuteReader<T>(string[] commands, Table<T> table, ParameterKeeper? keeper = null)
    {
        return ExecuteInTransaction((connection, transaction) =>
        {
            var results = new List<Record<T>>();
            foreach (var command in commands)
            {
                using var cmd = new SqliteCommand(command, connection, transaction);
                if (keeper != null && keeper.Parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(keeper.Parameters
                        .Select(x =>
                            new SqliteParameter(_generator.GetParameter(x.Value, keeper),
                                _generator.GetValue(x.Value))));
                }

                var reader = cmd.ExecuteReader();
                var pks = table.GetPks(); // hodnoty všech pk v tabulce
                var pk = table.Schema.GetPrimaryKeyAttribute(); // jméno pk sloupce
                ProcessReaderResults(reader, table, results, pks, pk);
            }

            return results;
        });
    }

    // projde výsledek Readeru a udělá z něho množinu záznamů
    private void ProcessReaderResults<T>(SqliteDataReader reader, Table<T> table, List<Record<T>> results, HashSet<object> pks, SchemaAttribute pk)
    {
        while (reader.Read())
        {
            var instance = Activator.CreateInstance<T>();
            var isDuplicate = false;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                // byl jsem chytrej a schéma atributy se jmenujou stejně jako sloupce
                var schemaAttribute = table.Schema.SchemaAttributes.First(x => x.Name == columnName);
                var value = Convert.ChangeType(reader.GetValue(i), schemaAttribute.Type);
                
                // jsem v pk sloupci a pk s touhle hodnotou už v paměti je
                // -> přeskočím záznam a do výsledků dám ten v paměti
                if (columnName == pk.Name && pks.Contains(value))
                {
                    // pokud už mám v tabulce Record kterej má stejný id, pak ho nahradím
                    isDuplicate = true;
                    results.Add(table.GetRecordByPk(value)); // tady by nemělo být null
                    break;
                }

                table.SetValue(schemaAttribute, instance, Convert.ChangeType(value, schemaAttribute.Type));
            }

            if (isDuplicate)
            {
                continue;
            }

            var record = new Record<T>(instance, RecordAction.NONE);
            table.Add(record);
            results.Add(record);
        }
    }

    public override void TryCreateTable<T>(Table<T> table)
    {
        ExecuteVoid([_generator.TryCreateTable(table)]);
    }

    public override void DropTable<T>(Table<T> table)
    {
        ExecuteVoid([_generator.DropTable(table)]);
    }

    public override void Insert<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper, out int lastNewId)
    {
        lastNewId = 0;
        var objects = ExecuteScalar([_generator.Insert(table, records, keeper), _generator.GetLastId()], keeper);
        TryParse(objects.Last()?.ToString(), out lastNewId);
    }

    public override List<Record<T>> Select<T>(Table<T> table, QueryRoot<T> root)
    {
        return ExecuteReader([_generator.Select(table, root)], table, root.Keeper);
    }

    public override void Upsert<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper)
    {
        ExecuteVoid([
            .. records.Select(x => _generator.Upsert(table, x, table.GetRecordPk(x), keeper))
        ], keeper);
    }

    public override void Delete<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper)
    {
        ExecuteVoid([
            .. records.Select(x => _generator.Delete(table, table.GetRecordPk(x)))
        ], keeper);
    }
}