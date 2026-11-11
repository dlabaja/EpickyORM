using EpickyORM.Core.Enums;

namespace EpickyORM.Core.Structures.Backend;

public class Record<T>
{
    public T Instance { get; }
    public RecordAction RecordAction { get; set; }

    public Record(T instance, RecordAction action)
    {
        Instance = instance;
        RecordAction = action;
    }

    public void ResetAction()
    {
        RecordAction = RecordAction.NONE;
    }
}