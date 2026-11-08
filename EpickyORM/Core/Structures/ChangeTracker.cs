using EpickyORM.Core.Enum;

namespace EpickyORM.Core.Structures;

public class ChangeTracker
{
    private Dictionary<object, RecordStatus> _trackedChanges = new Dictionary<object, RecordStatus>();
    
    public void Add(Record record, RecordStatus status)
    {
        if (_trackedChanges.ContainsKey(record.Instance))
        {
            _trackedChanges[record.Instance] = status;
            return;
        }
        _trackedChanges.Add(record, status);
    }

    public List<KeyValuePair<object, RecordStatus>> Flush()
    {
        var list = _trackedChanges.ToList();
        _trackedChanges.Clear();
        return list;
    }
}