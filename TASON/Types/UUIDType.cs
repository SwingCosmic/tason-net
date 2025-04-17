

using System.Text.RegularExpressions;
using TASON.Serialization;

namespace TASON.Types;

/// <summary>
/// TASON UUID类型信息
/// </summary>
public partial class UUIDType : TasonScalarTypeBase<Guid>
{


#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase)]
    private static partial Regex PatternRegex();
    private static readonly Regex pattern = PatternRegex();
#else
    private static readonly Regex pattern = new Regex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.IgnoreCase);
#endif
    protected override Guid DeserializeCore(string text, TasonSerializerOptions options)
    {
        if (!pattern.IsMatch(text))
        {
            throw new ArgumentException("Invalid UUID format", nameof(text));
        }
        return Guid.Parse(text);
    }

    protected override string SerializeCore(Guid value, TasonSerializerOptions options)
    {
        return value.ToString("D");
    }


}
