using System.Text.Json;
using System.Text.Json.Serialization;
using EpickyORM.Core.Enums;
using System.IO.Hashing;
using System.Text;

namespace EpickyORM.Core.Structures.Backend;

// tahle třída je důvod proč mám radši TypeScript, striktní typy jsou strašně omezující, pomoc
// ukládá všechny tabulky a otisky záznamů (objektů v nich)
public class ChangeTracker
{
    public List<dynamic> Tables => [.. _tables.Values];
    private Dictionary<Type, dynamic> _tables = new();
    // otisky objektů 
    private Dictionary<object, ulong> _snapshots = new();

    private JsonSerializerOptions _options = new()
    {
        IncludeFields = true,
        // místo cyklu nastaví problémové pole na null (při tvorbě otisku mi to nevadí)
        // spíš do budoucna, zatím nemám vazby a objekty proto ukládat nemůžu
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    // serializuje třídu a udělá z ní hash (šetří místo)
    // XxHash3 by měl být celkem rychlej a má jenom 64b
    private ulong Hash(object instance)
    {
        var json = JsonSerializer.Serialize(instance, _options);
        return XxHash3.HashToUInt64(Encoding.UTF8.GetBytes(json));
    }

    public void RegisterTable(object table)
    {
        _tables.Add(table.GetType().GenericTypeArguments[0], table);
    }
    
    public void Add<T>(Record<T> record)
    {
        if (!_tables.Values.Select(x => x.GetType().GenericTypeArguments[0]).Contains(typeof(T)))
        {
            throw new Exception($"There is no table for record of type {typeof(T)}");
        }
        GetTable<T>().Add(record);
    }

    // porovná hashe původního a aktuálního stavu instance a řekne, zda se objekt změnil
    public bool ObjectChanged(object instance)
    {
        if (!_snapshots.TryGetValue(instance, out var hash))
        {
            return false;
        }

        return hash != Hash(instance);
    }

    private Table<T> GetTable<T>()
    {
        return (Table<T>)_tables[typeof(T)];
    }

    // tohle se actually používá (v DbContext), ale přes reflexi takže to nejde vidět
    public List<Record<T>> GetRecords<T>(RecordAction action)
    {
        return GetTable<T>().GetRecords().Where(x => x.RecordAction == action).ToList();
    }

    // ošklivou generikou Flushne všechny záznamy ve všech tabulkách
    // používá se po uložení do DB
    public void Flush()
    {
        foreach (var table in Tables)
        {
            var tableType = table.GetType();
            var method = tableType.GetMethod("Flush")!;
            method.Invoke(table, null);
            
            foreach (var record in table.GetRecords())
            {
                if (record.RecordAction != RecordAction.NONE)
                {
                    continue;
                }
                
                if (!_snapshots.TryAdd(record.Instance, Hash(record.Instance)))
                {
                    _snapshots[record.Instance] = Hash(record.Instance);
                }
            }
        }
    }
}