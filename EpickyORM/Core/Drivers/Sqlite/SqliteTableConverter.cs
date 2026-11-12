using System.Globalization;
using EpickyORM.Core.Structures.Backend;
using static EpickyORM.Core.Utils.StringUtils;

namespace EpickyORM.Core.Drivers.SQLite;

public class SqliteTableConverter : TableConverter
{
    // https://www.sqlite.org/datatype3.html
    public override string ConvertType(TypeCode type)
    {
        switch (type)
        {
            // speciální null pro databáze
            case TypeCode.DBNull:
            case TypeCode.Empty:
                return "NULL";
            case TypeCode.String:
            case TypeCode.Char:
                return "TEXT";
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return "INTEGER";
            case TypeCode.Single:
            case TypeCode.Double:
                return "REAL";
            default:
                throw new Exception($"Cannot save {type} to database");
        }
    }

    public override string ConvertTableAttribute(SchemaAttribute schemaAttribute)
    {
        var pkText = schemaAttribute.IsPk ? "PRIMARY KEY" : "";
        return $"{schemaAttribute.Name} {ConvertType(Type.GetTypeCode(schemaAttribute.Type))} {pkText}".Trim();
    }

    public override string ConvertTable<T>(Table<T> table)
    {
        return $"{table.Name} ({string.Join(", ", table.Schema.SchemaAttributes.Select(ConvertTableAttribute))})";
    }

    public override string GetSchemaArray<T>(Table<T> table, bool noPk)
    {
        return $"({string.Join(", ", table.Schema.SchemaAttributes
            .Where(x => noPk ? !x.IsPk : true)
            .Select(x => x.Name))})";
    }

    public override string GetValuesArray<T>(Table<T> table, Record<T> record, ParameterKeeper keeper, bool noPk)
    {
        return $"({string.Join(", ",
            table.GetRecordValues(record)
                .Where(x => noPk ? !x.x.IsPk : true)
                .Select(x => x.Item2 == null ? "" : $"@{keeper.GetParamName(x.Item2)}"))})";
    }

    public override string GetMultipleValuesArray<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper, bool noPk)
    {
        return $"{string.Join(", ", records.Select(x => GetValuesArray(table, x, keeper, noPk)))}";
    }

    public override string FormatValue(object? obj)
    {
        return obj switch
        {
            string i => $"\"{i}\"",
            float i => i.ToString(provider: CultureInfo.InvariantCulture),
            double i => i.ToString(provider: CultureInfo.InvariantCulture),
            _ => obj?.ToString() ?? "NULL"
        };
    }

    public override string GetSetArray<T>(Table<T> table, Record<T> record, ParameterKeeper keeper)
    {
        return $"SET {string.Join(", ", table.Schema.SchemaAttributes.Select(x => GetSetItem(x, table.GetValue(x, record), keeper)))}";
    }

    public override string GetSetItem(SchemaAttribute attribute, object? value, ParameterKeeper keeper)
    {
        return $"{attribute.Name} = @{keeper.GetParamName(value)}";
    }
}