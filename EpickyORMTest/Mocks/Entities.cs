using EpickyORM.Core.Attributes;

namespace EpickyORMTest.Mocks;

class Predmet
{
    public string Code { get; set; }
    public Teacher Teacher { get; set; }
    [EpickyORM.Core.Attributes.Ignore] public string NotIncludedProp { get; set; }
    [Include] private string _includedField;
    private string _ignoredAutomatically;
    public string _ignoredAsWell;
}

class Teacher
{
    [PrimaryKey] public int Id { get; set; } 
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Titul { get; set; }
}