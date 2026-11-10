using System.Linq.Expressions;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures;
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

    public override string Select(QueryRoot root)
    {
        var whereClause = root.WhereClause.Parse(this);
        return $"SELECT * FROM  {whereClause}";
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
        return $"({binaryCondition.Left.Parse(this)} {GetBinaryOperator(binaryCondition.BinaryOperator)} {binaryCondition.Right.Parse(this)})";
    }

    public override string GetParameter(QueryParameter parameter)
    {
        return $"@p{parameter.Id}";
    }

    public override string GetWhereClause(QueryWhereClause whereClause)
    {
        return $"WHERE {whereClause.BinaryCondition.Parse(this)}";
    }

    public override string GetAttributeName(QueryAttributeName attributeName)
    {
        return $"{attributeName.Name}";
    }
}