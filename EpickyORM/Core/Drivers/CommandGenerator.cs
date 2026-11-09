using System.Linq.Expressions;
using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Linq.QueryModel;
using EpickyORM.Core.Structures.Linq.QueryModel.Clause;
using EpickyORM.Core.Structures.Linq.QueryModel.Primitive;

namespace EpickyORM.Core.Drivers;

public abstract class CommandGenerator
{
    public abstract string TryCreateTable(Table table);

    public abstract string DropTable(Table table);

    public abstract string Insert(Table table, List<Record> records);

    public abstract string GetLastId();
    
    public abstract string Select(QueryRoot root);

    public abstract string GetBinaryOperator(ExpressionType expressionType);
    
    public abstract string GetBinaryCondition(QueryBinaryCondition binaryCondition);

    public abstract string GetParameter(QueryParameter parameter);
    
    public abstract string GetWhereClause(QueryWhereClause whereClause);
}