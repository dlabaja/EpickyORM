using EpickyORM.Core.Structures;

namespace EpickyORM.Core.Drivers;

public abstract class CommandGenerator
{
    public abstract string CreateTable(Table table);

    public abstract string DropTable(Table table);
}