using EpickyORM.Core.Structures;
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
                // možná by bylo lepší to thrownout?
                return "BLOB";
        }
    }

    public override string ConvertTableAttribute(TableAttribute tableAttribute)
    {
        return $"{tableAttribute.Name.ToSnakeCase()} ${ConvertType(tableAttribute.Type)}";
    }

    public override string ConvertTable(Table table)
    {
        return $"{table.Name}({string.Join(", ", table.TableAttributes.Select(ConvertTableAttribute))})";
    }
}