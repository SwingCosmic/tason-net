

using System.Text.RegularExpressions;
using TASON.Serialization;
using TASON.Util;
using RO = System.Text.RegularExpressions.RegexOptions;
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
    protected override Regex DeserializeCore(string text, TasonSerializerOptions options)
    {
        var match = regexpPattern.Match(text);
        if (!match.Success)
        {
            throw new FormatException($"Invalid RegExp: {text}");
        }

        var pattern = match.Groups[1].Value;
        var flags = match.Groups[2].Value ?? "";

        var option = RO.None;
        foreach (var flag in flags) 
        {
            option |= flag switch 
            {
                'g' => RO.None,// 没有影响
                'i' => RO.IgnoreCase,
                'm' => RO.Multiline,
                'n' => RO.ExplicitCapture,
                's' => RO.Singleline,
                'x' => RO.IgnorePatternWhitespace,
                'u' => RO.None,// .NET始终支持Unicode
                'y' => RO.None,// 没有影响
                _ => throw new FormatException($"Flag {flag} is not supported")
            };
        }

        return new Regex(pattern, option);
    }

    protected override string SerializeCore(Regex value, TasonSerializerOptions options)
    {
        var s = value.ToString();
        if (inlineOptionPattern.IsMatch(s))
        {
            throw new NotSupportedException("Inline options or comment is not supported");
        }

        var flags = "";
        if (value.Options.HasFlagFast(RO.IgnoreCase))
            flags += 'i';
        if (value.Options.HasFlagFast(RO.Multiline))
            flags += 'm';
        if (value.Options.HasFlagFast(RO.ExplicitCapture))
            flags += 'n';
        if (value.Options.HasFlagFast(RO.Singleline))
            flags += 's';        
        if (value.Options.HasFlagFast(RO.IgnorePatternWhitespace))
            flags += 'x';
        if (value.Options.HasFlagFast(RO.RightToLeft))
            throw new NotSupportedException("RightToLeft is not supported");

        flags += 'u';// .NET始终支持Unicode
        return $"/{s}/{flags}";
    }

}