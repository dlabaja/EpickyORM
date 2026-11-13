using System.Linq.Expressions;
using System.Text;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures.Backend;
using EpickyORM.Core.Structures.Linq.QueryModel;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Drivers.Sqlite;

public class SqliteCommandGenerator : CommandGenerator
{
    private readonly SqliteTableConverter _converter = new();

    public override string TryCreateTable<T>(Table<T> table)
    {
        return $"CREATE TABLE IF NOT EXISTS {_converter.ConvertTable(table)};";
    }

    public override string DropTable<T>(Table<T> table)
    {
        return $"DROP TABLE {table.Name};";
    }

    public override string Insert<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper)
    {
        return
            $"INSERT INTO {table.Name} {_converter.GetSchemaArray(table, true)} VALUES {_converter.GetMultipleValuesArray(table, records, keeper, true)};";
    }

    public override string GetLastId()
    {
        // vrátí poslední vygenerované ID (PK) v connectionu!
        return "SELECT last_insert_rowid();";
    }

    public override string Select<T>(Table<T> table, QueryRoot<T> root)
    {
        var builder = new List<string?>
        {
            "SELECT",
            root.DistinctClause != null ? GetDistinctClause(root.DistinctClause) : "",
            $"* FROM {table.Name}",
            root.WhereClause != null ? GetWhereClause(root.WhereClause) : "",
            root.OrderByClause != null ? GetOrderByClause(root.OrderByClause) : "",
            GetLimitClause(root.LimitClause, root.Keeper),
            root.OffsetClause != null ? GetOffsetClause(root.OffsetClause, root.Keeper) : ""
        };

        return $"{string.Join(" ", builder)};";
    }

    // https://www.sqlitetutorial.net/sqlite-upsert/
    public override string Upsert<T>(Table<T> table, Record<T> record, object pk, ParameterKeeper keeper)
    {
        var builder = new List<string?>
        {
            $"INSERT INTO {table.Name}",
            _converter.GetSchemaArray(table, false),
            "VALUES",
            _converter.GetValuesArray(table, record, keeper, false),

            "ON CONFLICT",
            $"({table.Schema.GetPrimaryKeyAttribute().Name})",
            "DO UPDATE",
            _converter.GetSetArray(table, record, keeper),
            "WHERE",
            $"{table.Schema.GetPrimaryKeyAttribute().Name} = {_converter.FormatValue(pk)}"
        };

        return $"{string.Join(" ", builder)};";
    }

    public override string Delete<T>(Table<T> table, object pk)
    {
        var builder = new List<string?>
        {
            $"DELETE FROM {table.Name}",
            "WHERE",
            $"{table.Schema.GetPrimaryKeyAttribute().Name} = {_converter.FormatValue(pk)}"
        };

        return $"{string.Join(" ", builder)};";
    }

    public override string GetBinaryOperator(ExpressionType expressionType)
    {
        // tyhle nový switche se mi líběj
        return expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>", // nebo !=
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new Exception($"Unsupported operator {expressionType}")
        };
    }

    public override string GetBinaryCondition(QueryBinaryCondition binaryCondition)
    {
        return
            $"({binaryCondition.Left.Parse(this)} {GetBinaryOperator(binaryCondition.BinaryOperator)} {binaryCondition.Right.Parse(this)})";
    }

    public override string GetParameter(object? value, ParameterKeeper keeper)
    {
        return $"@{keeper.GetParamName(value)}";
    }

    public override string GetValue(object? value)
    {
        return _converter.FormatValue(value);
    }

    public override string GetWhereClause(QueryWhereClause whereClause)
    {
        return $"WHERE {whereClause.BinaryCondition.Parse(this)}";
    }

    public override string GetOrderByClause(QueryOrderByClause orderByClause)
    {
        var order = GetOrderByDirection(orderByClause.OrderDirection);
        return
            $"ORDER BY {string.Join(", ", orderByClause.AttributeNames.Names.Select(x =>
                $"{x.Name} {order}"))}";
    }

    public override string GetDistinctClause(QueryDistinctClause distinctClause)
    {
        return "DISTINCT";
    }

    public override string GetLimitClause(QueryLimitClause? limitClause, ParameterKeeper keeper)
    {
        return limitClause != null 
            ? $"LIMIT {GetParameter(limitClause.Limit.Value, keeper)}" 
            : "LIMIT -1"; // bez tohodle by nefungoval Offset, zároveň to ničemu nevadí
    }

    public override string GetOffsetClause(QueryOffsetClause offsetClause, ParameterKeeper keeper)
    {
        return $"OFFSET {GetParameter(offsetClause.OffsetCount.Value, keeper)}";
    }

    public override string GetAttributeName(QueryAttributeName attributeName)
    {
        return $"{attributeName.Name}";
    }

    public override string GetAttributeNames(QueryAttributeNames attributeNames)
    {
        return $"{string.Join(", ", attributeNames.Names.Select(GetAttributeName))}";
    }

    public override string GetOrderByDirection(OrderByDirection direction)
    {
        return direction switch
        {
            OrderByDirection.ASCENDING => "ASC",
            OrderByDirection.DESCENDING => "DESC",
            _ => throw new Exception($"Unknown OrderBy direction {direction}")
        };
    }
}