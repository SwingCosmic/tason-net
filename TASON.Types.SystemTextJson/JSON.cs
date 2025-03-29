using System.Numerics;
using System.Text.Json;
using TASON.Serialization;

namespace TASON.Types.SystemTextJson;

/// <summary>
/// 表示一个JSON对象
/// </summary>
public class JSON : IEquatable<JSON>, ITasonTypeDiscriminator
#if NET7_0_OR_GREATER
    , IEqualityOperators<JSON, JSON, bool>
#endif
{
    readonly JsonSerializerOptions options;
    public JSON(JsonSerializerOptions options, JSONSubType subType = JSONSubType.All)
    {
        this.options = options;
        SubType = subType;
        JsonString = subType == JSONSubType.Array ? "[]" : 
            subType == JSONSubType.Object ? "{}" : "null";
    }
    
    public JSON(string json, JsonSerializerOptions options, JSONSubType subType = JSONSubType.All)
    {
        this.options = options;
        JsonString = json.Trim();
        SubType = subType;
        CheckSubType();
    }
    
    public JSON(object? obj, JsonSerializerOptions options, JSONSubType subType = JSONSubType.All)
    {
        this.options = options;
        JsonString = JsonSerializer.Serialize(obj, options);
        SubType = subType;
        CheckSubType();
    }

    /// <summary>JSON字符串值</summary>
    public string JsonString { get; set; }
    /// <summary>JSON子类型</summary>
    public JSONSubType SubType { get; }

    /// <summary>反序列化JSON字符串为.NET对象</summary>
    public T? GetValue<T>() => JsonSerializer.Deserialize<T>(JsonString, options);

    void CheckSubType()
    {
        if (SubType == JSONSubType.Array && !JsonString.StartsWith('['))
        {
            throw new ArgumentException("value is not a valid JSONArray");
        }
        if (SubType == JSONSubType.Object && !JsonString.StartsWith('{'))
        {
            throw new ArgumentException("value is not a valid JSONObject");
        }
    }

    /// <inheritdoc/>
    public bool Equals(JSON? other)
    {
        if (other is null) return false;
        return JsonString == other.JsonString && SubType == other.SubType;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as JSON);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(JsonString, SubType);
    }

    /// <summary>根据<see cref="SubType"/>返回实际的TASON类型名称</summary>
    public string GetTypeName()
    {
        return SubType switch
        {
            JSONSubType.Array => "JSONArray",
            JSONSubType.Object => "JSONObject",
            _ => "JSON",
        };
    }

    /// <inheritdoc/>
    public static bool operator ==(JSON? left, JSON? right)
    {
        return (left, right) switch
        {
            (null, null) => true,
            (null, _) or (_, null) => false,
            _ => left.Equals(right),
        };
    }

    /// <inheritdoc/>
    public static bool operator !=(JSON? left, JSON? right) => !(left == right);
}


/// <summary>JSON子类型</summary>
public enum JSONSubType
{
    /// <summary>全部</summary>
    All,
    /// <summary>JSON对象</summary>
    Object,
    /// <summary>JSON数组</summary>
    Array,
}