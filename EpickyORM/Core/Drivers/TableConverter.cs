using EpickyORM.Core.Structures;
using EpickyORM.Core.Structures.Backend;

namespace EpickyORM.Core.Drivers;

public abstract class TableConverter
{
    public abstract string ConvertType(TypeCode type);

    public abstract string ConvertTableAttribute(SchemaAttribute schemaAttribute);
    
    public abstract string ConvertTable<T>(Table<T> table);

    public abstract string GetSchemaArray<T>(Table<T> table);

    public abstract string GetValuesArray<T>(Table<T> table, Record<T> record);

    public abstract string GetMultipleValuesArray<T>(Table<T> table, List<Record<T>> records);

    public abstract string FormatValue(Object obj);

    public abstract string GetSetArray<T>(Table<T> table, Record<T> record);
    
    public abstract string GetSetItem(SchemaAttribute attribute, int paramId);
}