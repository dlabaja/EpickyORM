using System.Linq.Expressions;
using EpickyORM.Core.Enums;
using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Backend;
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
    
    public abstract string Select(Table table, QueryRoot root);
    
    public abstract string Update(Table table, Record record, object pk);
    
    public abstract string Delete(Table table, object pk);

    public abstract string GetBinaryOperator(ExpressionType expressionType);
    
    public abstract string GetBinaryCondition(QueryBinaryCondition binaryCondition);

    public abstract string GetParameter(QueryParameter parameter);
    
    public abstract string GetWhereClause(QueryWhereClause whereClause);
    
    public abstract string GetOrderByClause(QueryOrderByClause orderByClause);

    public abstract string GetDistinctClause(QueryDistinctClause distinctClause);
    
    public abstract string GetLimitClause(QueryLimitClause limitClause);

    public abstract string GetOffsetClause(QueryOffsetClause offsetClause);
    
    public abstract string GetAttributeName(QueryAttributeName attributeName);
    
    public abstract string GetAttributeNames(QueryAttributeNames attributeNames);
    
    public abstract string GetOrderByDirection(OrderByDirection direction);
}