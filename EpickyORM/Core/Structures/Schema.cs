namespace EpickyORM.Core.Structures;

public class Schema
{
    public List<SchemaAttribute> SchemaAttributes { get; }
    
    public Schema(List<SchemaAttribute> schemaAttributes)
    {
        SchemaAttributes = schemaAttributes;
    }
}