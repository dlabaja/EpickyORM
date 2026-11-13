using EpickyORM.Core.Drivers;

namespace EpickyORM.Core.Structures.Linq.QueryModel;

// abstraktní třída pro tvorbu Expression stromu
// inspiroval jsem se svým Lisp interpreterem, kde jsem řešil podobnej problém
// https://github.com/dlabaja/SharpLisp
// (rozhodně ne reklama) :) 
public abstract class QueryExpression
{
    public abstract string Parse(CommandGenerator generator);
}