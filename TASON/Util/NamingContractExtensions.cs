using System.Text.RegularExpressions;
using TASON.Serialization;

namespace TASON.Util;

/// <summary>
/// 提供命名约定转换相关的扩展方法
/// </summary>
public static partial class NamingContractExtensions
{
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"(^|_|-|\s)(\w)")]
    private static partial Regex pascalRegex();
    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex splitRegex();

    private static Regex pascalPattern = pascalRegex();
    private static Regex splitPattern = splitRegex();
#else
    private static Regex pascalPattern = new Regex(@"(^|_|-|\s)(\w)");
    private static Regex splitPattern = new Regex(@"([a-z0-9])([A-Z])");
#endif    
    
    /// <summary>
    /// 将字符串转为指定命名约定的形式
    /// </summary>
    /// <param name="input">待转换的字符串</param>
    /// <param name="namingPolicy">命名策略</param>
    public static string ToCase(this string input, TasonNamingPolicy namingPolicy)
    {
        return namingPolicy switch
        {
            TasonNamingPolicy.CamelCase => input.ToCamelCase(),
            TasonNamingPolicy.PascalCase => input.ToPascalCase(),
            TasonNamingPolicy.SnakeCase => input.ToSnakeCase(),
            TasonNamingPolicy.KebabCase => input.ToKebabCase(),
            _ => input
        };
    }

    /// <summary>
    /// 将字符串转为<see cref="TasonNamingPolicy.PascalCase"/>
    /// </summary>
    /// <param name="input">待转换的字符串</param>
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        return pascalPattern.Replace(input, match => match.Groups[2].Value.ToUpper());
    }

    /// <summary>
    /// 将字符串转为<see cref="TasonNamingPolicy.CamelCase"/>
    /// </summary>
    /// <param name="input">待转换的字符串</param>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        string pascalCase = ToPascalCase(input);
        return char.ToLower(pascalCase[0]) + pascalCase[1..];
    }

    /// <summary>
    /// 将字符串转为<see cref="TasonNamingPolicy.SnakeCase"/>
    /// </summary>
    /// <param name="input">待转换的字符串</param>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        return splitPattern.Replace(input, "$1_$2")
                    .Replace("-", "_")
                    .Replace(" ", "_")
                    .ToLower();
    }

    /// <summary>
    /// 将字符串转为<see cref="TasonNamingPolicy.KebabCase"/>
    /// </summary>
    /// <param name="input">待转换的字符串</param>
    public static string ToKebabCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        return splitPattern.Replace(input, "$1-$2")
                    .Replace("_", "-")
                    .Replace(" ", "-")
                    .ToLower();
    }

}
