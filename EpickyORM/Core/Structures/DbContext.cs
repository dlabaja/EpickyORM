namespace EpickyORM.Core.Structures;

public abstract class DbContext
{
    private ChangeTracker _changeTracker = new ChangeTracker();

    public DbContext()
    {
        RegisterTracker();
    }

    private void RegisterTracker()
    {
        // this vrací aktuální instanci -> potomka téhle třídy
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var dbSetInstance = property.GetValue(this);
            var field = property.PropertyType.GetFields().FirstOrDefault(x => x.Name == "_changeTracker");
            if (field == null)
            {
                continue;
            }
            field.SetValue(dbSetInstance, _changeTracker);
        }
    }
    
    public void SaveChanges()
    {
        
    }
}