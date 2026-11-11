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
    public abstract string TryCreateTable<T>(Table<T> table);

    public abstract string DropTable<T>(Table<T> table);

    public abstract string Insert<T>(Table<T> table, List<Record<T>> records);

    public abstract string GetLastId();
    
    public abstract string Select<T>(Table<T> table, QueryRoot<T> root);
    
    public abstract string Upsert<T>(Table<T> table, Record<T> record, object pk);
    
    public abstract string Delete<T>(Table<T> table, object pk);

    public abstract string GetBinaryOperator(ExpressionType expressionType);
    
    public abstract string GetBinaryCondition(QueryBinaryCondition binaryCondition);

    public abstract string GetParameter(string name);
    
    public abstract string GetParameter(QueryParameter param);
    
    public abstract string GetValue(object value);
    
    public abstract string GetWhereClause(QueryWhereClause whereClause);
    
    public abstract string GetOrderByClause(QueryOrderByClause orderByClause);

    public abstract string GetDistinctClause(QueryDistinctClause distinctClause);
    
    public abstract string GetLimitClause(QueryLimitClause limitClause);

    public abstract string GetOffsetClause(QueryOffsetClause offsetClause);
    
    public abstract string GetAttributeName(QueryAttributeName attributeName);
    
    public abstract string GetAttributeNames(QueryAttributeNames attributeNames);
    
    public abstract string GetOrderByDirection(OrderByDirection direction);
}