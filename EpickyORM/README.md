# EpickýORM
(kdyby byl průměrný tak ho nikdo nepoužívá)

Volně inspirováno EF Core (Context, DbSet, ...)

Funguje s SQLite (./database.sqlite), ale ORM je driver-independant a pro jiné databáze stačí dopsat tři třídy v Driverech

Podporuje základní LINQ (Where, Distinct, Limit, Skip, OrderBy), další jdou celkem jednoduše přidat

Select vždy vrací celý řádek (\*), takže Distinct vlastně nedělá nic protože všechny řádky jsou díky Pk unikátní*

Chtěl jsem přidat i podporu pro vazby (ukládání objektů v objektech), ale po týdnu v kuse co programuju už na to nemám sílu