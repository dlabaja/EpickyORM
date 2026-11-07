using System.Text.Json;

namespace EpickyORM.Core.Utils;

public static class StringUtils
{
    // c# 14 novinka
    extension(string str)
    {
        public string ToCamelCase()
        {
            return JsonNamingPolicy.SnakeCaseLower.ConvertName(str);
        }

        public string ToSnakeCase()
        {
            return JsonNamingPolicy.CamelCase.ConvertName(str);
        }
    }
}