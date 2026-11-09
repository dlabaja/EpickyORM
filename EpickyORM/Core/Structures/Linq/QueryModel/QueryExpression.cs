using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel;

public abstract class QueryExpression
{
    public abstract string Parse(CommandGenerator generator);
}