namespace EpickyORM.Core.Structures;

public abstract class DbContext
{
    private ChangeTracker _changeTracker = new ChangeTracker();

    protected DbContext()
    {
        CheckDbSets();
        CreateDbSets();
        RegisterTracker();
    }

    private void CheckDbSets()
    {
        var properties = GetType().GetProperties();
        var invalidProperties = properties
            .Where(x => x.PropertyType != typeof(DbSet<>));
        if (invalidProperties.Any())
        {
            throw new Exception($"Class {GetType()} can only have properties of DbSet<class> type.");
        }

        var invalidAccessors = properties.Where(x => x.GetMethod == null || x.SetMethod != null);
        if (invalidAccessors.Any())
        {
            throw new Exception($"Properties of {GetType()} must have only get accessors.");
        }
    }
    
    private void CreateDbSets()
    {
        var properties = GetType().GetProperties();
        var invalidProperties = properties
            .Where(x => x.PropertyType != typeof(DbSet<>));
        if (invalidProperties.Any())
        {
            throw new Exception($"Class {GetType()} can only have properties of DbSet<class> type.");
        }
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
        // todo projdi changeTracker, vygeneruj příkaz a pošli to driveru
    }
}