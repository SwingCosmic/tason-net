

using System.Text.RegularExpressions;

namespace TASON.Types;

/// <summary>
/// TASON UUID类型信息
/// </summary>
public partial class UUIDType : TasonScalarTypeBase<Guid>
{
    static readonly Regex pattern = PatternRegex();
    protected override Guid DeserializeCore(string text, SerializerOptions options)
    {
        if (!pattern.IsMatch(text))
        {
            throw new ArgumentException("Invalid UUID format", nameof(text));
        }
        return Guid.Parse(text);
    }

    protected override string SerializeCore(Guid value, SerializerOptions options)
    {
        return value.ToString("D");
    }

    [GeneratedRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase)]
    private static partial Regex PatternRegex();
}
