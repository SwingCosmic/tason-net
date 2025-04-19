using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TASON.Types.NewtonsoftJson;

/// <summary>
/// 表示一个JSON对象
/// </summary>
public sealed record class JSONNewtonsoft : JSON<JsonSerializerSettings>, IEquatable<JSONNewtonsoft>
#if NET7_0_OR_GREATER
    , IEqualityOperators<JSONNewtonsoft, JSONNewtonsoft, bool>
#endif
{

    /// <inheritdoc/>
    public JSONNewtonsoft(JsonSerializerSettings options, JSONSubType subType = JSONSubType.All) : base(options, subType)
    {
    }

    /// <inheritdoc/>
    public JSONNewtonsoft(string json, JsonSerializerSettings options, JSONSubType subType = JSONSubType.All) : base(json, options, subType)
    {
    }

    /// <inheritdoc/>
    public JSONNewtonsoft(object? obj, JsonSerializerSettings options, JSONSubType subType = JSONSubType.All) : base(obj, options, subType)
    {
    }


    /// <inheritdoc/>
    public override void CheckSyntax(string json, JsonSerializerSettings options)
    {
        _ = JToken.Parse(json);
    }

    /// <inheritdoc/>
    public override T Deserialize<T>(string json, JsonSerializerSettings options)
    {
        return JsonConvert.DeserializeObject<T>(json, options)!;
    }

    /// <inheritdoc/>
    public override string Serialize<T>(T obj, JsonSerializerSettings options)
    {
        return JsonConvert.SerializeObject(obj, options);
    }
}
