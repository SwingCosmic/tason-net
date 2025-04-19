using System.Numerics;
using System.Text.Json;

namespace TASON.Types.SystemTextJson;

/// <summary>
/// 表示一个JSON对象
/// </summary>
public sealed record class JSONSystemText : JSON<JsonSerializerOptions>, IEquatable<JSONSystemText>
#if NET7_0_OR_GREATER
    , IEqualityOperators<JSONSystemText, JSONSystemText, bool>
#endif
{

    /// <inheritdoc/>
    public JSONSystemText(JsonSerializerOptions options, JSONSubType subType = JSONSubType.All) : base(options, subType)
    {
    }

    /// <inheritdoc/>
    public JSONSystemText(string json, JsonSerializerOptions options, JSONSubType subType = JSONSubType.All) : base(json, options, subType)
    {
    }

    /// <inheritdoc/>
    public JSONSystemText(object? obj, JsonSerializerOptions options, JSONSubType subType = JSONSubType.All) : base(obj, options, subType)
    {
    }


    /// <inheritdoc/>
    public override void CheckSyntax(string json, JsonSerializerOptions options)
    {
        _ = JsonDocument.Parse(json, options.GetDocumentOptions()); 
    }

    /// <inheritdoc/>
    public override T Deserialize<T>(string json, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(json, options)!;
    }

    /// <inheritdoc/>
    public override string Serialize<T>(T obj, JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(obj, options);
    }
}
