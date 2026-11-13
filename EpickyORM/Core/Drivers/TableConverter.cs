using EpickyORM.Core.Structures.Backend;

namespace EpickyORM.Core.Drivers;

// převádí tabulku, schéma a atributy na části SQL dotazů
public abstract class TableConverter
{
    public abstract string ConvertType(TypeCode type);

    public abstract string ConvertTableAttribute(SchemaAttribute schemaAttribute);
    
    public abstract string ConvertTable<T>(Table<T> table);

    public abstract string GetSchemaArray<T>(Table<T> table, bool noPk);

    public abstract string GetValuesArray<T>(Table<T> table, Record<T> record, ParameterKeeper keeper, bool noPk);

    public abstract string GetMultipleValuesArray<T>(Table<T> table, List<Record<T>> records, ParameterKeeper keeper, bool noPk);

    public abstract string FormatValue(object obj);

    public abstract string GetSetArray<T>(Table<T> table, Record<T> record, ParameterKeeper keeper);
    
    public abstract string GetSetItem(SchemaAttribute attribute, object value, ParameterKeeper keeper);
}