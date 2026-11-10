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

    public override string TryCreateTable(Table table)
    {
        return $"CREATE TABLE IF NOT EXISTS {_converter.ConvertTable(table)};";
    }

    public override string DropTable(Table table)
    {
        return $"DROP TABLE {table.Name};";
    }

    public override string Insert(Table table, List<Record> records)
    {
        return
            $"INSERT INTO {table.Name} {_converter.GetSchemaArray(table)} VALUES {_converter.GetMultipleValuesArray(table, records)};";
    }

    public override string GetLastId()
    {
        // vrátí poslední vygenerované ID (PK) v connectionu!
        return "SELECT last_insert_rowid();";
    }

    public override string Select(Table table, QueryRoot root)
    {
        var builder = new List<string?>();
        builder.Add("SELECT");
        builder.Add(root.DistinctClause != null ? GetDistinctClause(root.DistinctClause) : "");
        builder.Add($"* FROM {table.Name}");
        builder.Add(root.WhereClause != null ? GetWhereClause(root.WhereClause) : "");
        builder.Add(root.OrderByClause != null ? GetOrderByClause(root.OrderByClause) : "");
        builder.Add(root.LimitClause != null ? GetLimitClause(root.LimitClause) : "");
        builder.Add(root.OffsetClause != null ? GetOffsetClause(root.OffsetClause) : "");

        return $"{string.Join(" ", builder)};";
    }

    public override string Update(Table table, Record record, object pk)
    {
        var builder = new List<string?>();
        builder.Add($"UPDATE {table.Name}");
        builder.Add(_converter.GetSetArray(table, record));
        builder.Add("WHERE");
        builder.Add($"{table.Schema.GetPrimaryKeyAttribute().Name} = {_converter.FormatValue(pk)}");

        return $"{string.Join(" ", builder)};";
    }

    public override string Delete(Table table, object pk)
    {
        var builder = new List<string?>();
        builder.Add($"DELETE FROM {table.Name}");
        builder.Add("WHERE");
        builder.Add($"{table.Schema.GetPrimaryKeyAttribute().Name} = {_converter.FormatValue(pk)}");

        return $"{string.Join(" ", builder)};";
    }

    public override string GetBinaryOperator(ExpressionType expressionType)
    {
        // tyhle nový switche se mi líběj
        return expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>", // nebo i !=
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

    public override string GetParameter(QueryParameter parameter)
    {
        return $"@p{parameter.Id}";
    }

    public override string GetWhereClause(QueryWhereClause whereClause)
    {
        return $"WHERE {whereClause.BinaryCondition.Parse(this)}";
    }

    public override string GetOrderByClause(QueryOrderByClause orderByClause)
    {
        return
            $"ORDER BY {orderByClause.AttributeNames.Parse(this)} {GetOrderByDirection(orderByClause.OrderDirection)}";
    }

    public override string GetDistinctClause(QueryDistinctClause distinctClause)
    {
        return "DISTINCT";
    }

    public override string GetLimitClause(QueryLimitClause limitClause)
    {
        return $"LIMIT {GetParameter(limitClause.Limit)}";
    }

    public override string GetOffsetClause(QueryOffsetClause offsetClause)
    {
        return $"OFFSET {GetParameter(offsetClause.OffsetCount)}";
    }

    public override string GetAttributeName(QueryAttributeName attributeName)
    {
        return $"{attributeName.Name}";
    }

    public override string GetAttributeNames(QueryAttributeNames attributeNames)
    {
        return $"{string.Join(", ", attributeNames.Names)}";
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