using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers;

public abstract class TableConverter
{
    public abstract string ConvertType(TypeCode type);

    public abstract string ConvertTableAttribute(SchemaAttribute schemaAttribute);
    
    public abstract string ConvertTable(Table table);

    public abstract string GetSchemaArray(Table table);

    public abstract string GetValuesArray(Table table, Record record);

    public abstract string GetMultipleValuesArray(Table table, List<Record> records);
}