

using System.Text.RegularExpressions;
using TASON.Serialization;

namespace TASON.Types;

public partial class RegExpType : TasonScalarTypeBase<Regex>
{

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^/(.+)/([gimnsxuy]*)$")]
    private static partial Regex Pattern();

    // 不精确匹配，仅快速检查存在内联选项或者内联注释的正则表达式
    [GeneratedRegex(@"\(\?(#|[imnsx-]+)")]
    private static partial Regex InlineOptionPattern();

    private static Regex regexpPattern = Pattern();
    private static Regex inlineOptionPattern = InlineOptionPattern();
#else
    private static Regex regexpPattern = new Regex(@"^/(.+)/([gimnsxuy]*)$");
    private static Regex inlineOptionPattern = new Regex(@"\(\?(#|[imnsx-]+)");
#endif
    protected override Regex DeserializeCore(string text, SerializerOptions options)
    {
        var match = regexpPattern.Match(text);
        if (!match.Success)
        {
            throw new FormatException($"Invalid RegExp: {text}");
        }

        var pattern = match.Groups[1].Value;
        var flags = match.Groups[2].Value ?? "";

        var option = RegexOptions.None;
        foreach (var flag in flags) 
        {
            option |= flag switch 
            {
                'g' => RegexOptions.None,// 没有影响
                'i' => RegexOptions.IgnoreCase,
                'm' => RegexOptions.Multiline,
                'n' => RegexOptions.ExplicitCapture,
                's' => RegexOptions.Singleline,
                'x' => RegexOptions.IgnorePatternWhitespace,
                'u' => RegexOptions.None,// .NET始终支持Unicode
                'y' => RegexOptions.None,// 没有影响
                _ => throw new FormatException($"Flag {flag} is not supported")
            };
        }

        return new Regex(pattern, option);
    }

    protected override string SerializeCore(Regex value, SerializerOptions options)
    {
        var s = value.ToString();
        if (inlineOptionPattern.IsMatch(s))
        {
            throw new NotSupportedException("Inline options or comment is not supported");
        }

        var flags = "";
        if (value.Options.HasFlag(RegexOptions.IgnoreCase))
            flags += 'i';
        if (value.Options.HasFlag(RegexOptions.Multiline))
            flags += 'm';
        if (value.Options.HasFlag(RegexOptions.ExplicitCapture))
            flags += 'n';
        if (value.Options.HasFlag(RegexOptions.Singleline))
            flags += 's';        
        if (value.Options.HasFlag(RegexOptions.IgnorePatternWhitespace))
            flags += 'x';
        if (value.Options.HasFlag(RegexOptions.RightToLeft))
            throw new NotSupportedException("RightToLeft is not supported");

        flags += 'u';// .NET始终支持Unicode
        return $"/{s}/{flags}";
    }

}