namespace EpickyORM.Core.Structures.Backend;

// schéma databáze
public class Schema
{
    public List<SchemaAttribute> SchemaAttributes { get; }
    
    public Schema(List<SchemaAttribute> schemaAttributes)
    {
        SchemaAttributes = schemaAttributes;
    }

    public SchemaAttribute GetPrimaryKeyAttribute()
    {
        return SchemaAttributes.FirstOrDefault(x => x.IsPk) ??
               throw new Exception("Schema needs to have a primary key");
    }
}