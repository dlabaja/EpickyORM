using EpickyORM.Core.Enum;

namespace EpickyORM.Core.Structures;

public class Record
{
    public object Instance { get; }
    public Type Type { get; }
    public RecordAction RecordAction { get; private set; }

    public Record(object instance, Type type, RecordAction action)
    {
        Instance = instance;
        Type = type;
        RecordAction = action;
    }

    public void ResetAction()
    {
        RecordAction = RecordAction.NONE;
    }
}