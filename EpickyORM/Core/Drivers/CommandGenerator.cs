using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers;

public abstract class CommandGenerator
{
    public abstract string TryCreateTable(Table table);

    public abstract string DropTable(Table table);

    public abstract string Insert(Table table, List<Record> records);
}