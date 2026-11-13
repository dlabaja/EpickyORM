using EpickyORM.Core.Attributes;
using EpickyORM.Core.Drivers.SQLite;
using EpickyORM.Core.Structures.Frontend;

// Schéma DB, stejně jako EF dědí z DbContext
// vytvoří všechny DbSety, inicializuje tabulky (+ odvodí schémata) a zkusí je vytvořit v databázi pokud tam nejsou
class MyContext : DbContext
{
    // DbSet drží tabulku, jde nad ním volat LINQ (generuje SQL), jde nad ním přidávat a mazat z tabulky
    public DbSet<Teacher> Teachers { get; }
}

class Teacher
{
    // Bez PK atributu se DbContext odmítne vytvořit
    // může být libovolný typ, int je ale nejlepší
    [PrimaryKey] public int Id { get; set; } 
    public string Name { get; set; }
    // Include polí (defaultně ukládá jen Properties)
    [Include] public string ukladaSe;
    // Ignore properties
    [Ignore] public string NeukladaSe { get; set; }
}

public class Program
{
    public static void Main()
    {
        var path = "database.sqlite";
        File.Delete(path);
        
        // umožňuje přístup ke kontextu který vytvoří
        var db = new Database<MyContext>(new SqliteDatabaseDriver($"Data Source={path}"));
        
        // Pk = 0 -> db vloží nový záznam a vrátí nové Id (autoincrement) => Id na objektu se po savu aktualizuje
        // Pk > 0 -> db zkusí vložit nový záznam, pokud už pk existuje tak updatuje existující => UPSERT
        var teacher = new Teacher {Id = 0, Name = "Jan", ukladaSe = "test", NeukladaSe = "sus"};
        
        // přidá záznam do changeTrackeru
        db.Context.Teachers.Add(teacher);
        // přečte tracker (našel ADD díky pk=0), vloží přes INSERT 
        db.Context.SaveChanges();
        
        teacher.NeukladaSe = "sus2";
        // objekt je pořád uloženej v paměti, ale v trackeru má NONE (beze změny)
        // SaveChanges projde všechny NONE, serializuje instanci do JSON a porovná hash původní a aktuální instance
        // hashe se liší (i když jsem změnil ignorovanou prop), takže pošle příkaz do DB
        // příkaz nic neudělá, protože změněný prop není ve schématu -> data se přepíšou stejnými daty
        db.Context.SaveChanges();

        teacher.Name = "Jana";
        // stejné jako minule
        // tentokrát se ale změnila hodnota nad schématem, takže bude jiná query -> hodnota se zapíše
        db.Context.SaveChanges();

        // můžem si ověřit jestli se data opravdu změnila
        // tohle je reálný LINQ, dědím z IQueryable rozhraní a parsuju ho v docela šílené tree struktuře
        // ten LINQ podporuje Where, Take, Skip, Distinct, Order a OrderBy (viz testy)
        // ToList (nebo jiná execute metoda) vygeneruje SELECT příkaz a pošle ho DB, od téhle chvíle už pracujeme s normální kolekcí v paměti
        // aby mi SELECT nevytvořil duplicitní záznam, záznamy se stejným ID co držím v paměti přeskakuju
        var selected = db.Context.Teachers.Where(x => x.Name == "Jana").ToList();
        Console.WriteLine($"Našel jsem {selected.Count} záznamů s Name=Jana");

        selected[0].Name = "Bořek";
        db.Context.SaveChanges();
        
        // Jana už tam nebude
        selected = db.Context.Teachers.Where(x => x.Name == "Jana").ToList();
        Console.WriteLine($"Našel jsem {selected.Count} záznamů s Name=Jana");
        
        // Bořek ano
        selected = db.Context.Teachers.Where(x => x.Name == "Bořek").ToList();
        Console.WriteLine($"Našel jsem {selected.Count} záznamů s Name=Bořek");
        
        // Instance jde samozřejmě i smazat
        // můžu použít původní instanci, jsou to stejné reference
        Console.WriteLine($"Jsou to stejné instance? {teacher == selected[0]}");
        
        // v changeTrackeru se záznam označí jako REMOVE
        db.Context.Teachers.Remove(teacher); // nebo selected[0]
        db.Context.SaveChanges();
        
        // tady už prvek nenajdu, teď bych ho musel zase přes kontext přidat
        selected = db.Context.Teachers.Where(x => x.Name == "Bořek").ToList();
        Console.WriteLine($"Našel jsem {selected.Count} záznamů s Name=Bořek");
        
        // 1 AM notes:
        // pokud by někdo chtěl přidat jinou databázi než SQLite
        // stačí vytvořit novou složku v Drivers a dědit ze tří abstraktních tříd
        
        
        // snažil jsem se většinu věcí odchytit v testech
        // je možné že mi něco uniklo, ale jinak by tenhle EF Core z Lidlu měl být stabilní
        
        // všechny příkazy používají parametry, takže jsou odolné proti SQL Injection
        // taky používají transactiony a při erroru se rollbacknou
        
        // Update (Upsert) i Delete používají Where(pkName = pk), takže už nehrozí problémy na produkci :)
        
        // nejvíc jsem bojoval s typovým systémem
        // Table, Record i DbSet jsou generické, ChangeTracker ne (ukládá seznam tabulek s různou generikou)
        // několikrát se mi stalo že jsem to chtěl radší přepsat do TypeScriptu (tam je any)
        // nakonec jsem to nějak naprasil přes dynamic a reflexi
        
        // chtěl jsem implementovat i vazby, ale po týdnu koukání do Rideru už to mentálně nezvládám
    }
}