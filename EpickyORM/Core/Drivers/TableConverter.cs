using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers;

public abstract class TableConverter
{
    public abstract string ConvertType(TypeCode type);

    public abstract string ConvertTableAttribute(TableAttribute tableAttribute);

    public abstract string ConvertTable(Table table);
}