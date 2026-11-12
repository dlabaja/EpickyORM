using System.Text.Json;

namespace EpickyORM.Core.Utils;

public static class StringUtils
{
    public static string ToCamelCase(this string str)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(str);
    }

    public static string ToSnakeCase(this string str)
    {
        return JsonNamingPolicy.SnakeCaseLower.ConvertName(str);
    }
}