namespace EpickyORM.Core.Structures;

// množina/kolekce záznamů
public class DbSet<T>
{
    // inicializuje se přes reflexi v DbContext
    private ChangeTracker? _changeTracker;
    
    public void Add(T instance)
    {
        
    }
    
    
}