using EpickyORM.Core.Enum;

namespace EpickyORM.Core.Structures;

public class ChangeTracker
{
    private Dictionary<object, RecordAction> _trackedChanges = new Dictionary<object, RecordAction>();
    
    public void Add(Record record, RecordAction action)
    {
        if (_trackedChanges.ContainsKey(record.Instance))
        {
            _trackedChanges[record.Instance] = action;
            return;
        }
        _trackedChanges.Add(record, action);
    }

    public List<KeyValuePair<object, RecordAction>> Flush()
    {
        var list = _trackedChanges.ToList();
        _trackedChanges.Clear();
        return list;
    }
}