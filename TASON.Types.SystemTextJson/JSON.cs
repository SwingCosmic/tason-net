using System.Numerics;
using System.Text.Json;
using TASON.Serialization;

namespace TASON.Types.SystemTextJson;

/// <summary>
/// 表示一个JSON对象
/// </summary>
public sealed record class JSON : IEquatable<JSON>, ITasonTypeDiscriminator
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
        JsonString = json;
        SubType = subType;
    }
    
    public JSON(object? obj, JsonSerializerOptions options, JSONSubType subType = JSONSubType.All)
    {
        this.options = options;
        JsonString = JsonSerializer.Serialize(obj, options);
        SubType = subType;
    }

    private string jsonString;

    /// <summary>JSON字符串值</summary>
    public string JsonString 
    {
        get => jsonString;
        set
        {
            var text = value.Trim();
            CheckSubType(text);
            // 仅检查JSON字符串是否合法，丢弃结果
            _ = JsonDocument.Parse(text);
            jsonString = text;
        }
    }

    /// <summary>JSON子类型</summary>
    public JSONSubType SubType { get; }

    /// <summary>反序列化JSON字符串为.NET对象</summary>
    public T? GetValue<T>() => JsonSerializer.Deserialize<T>(JsonString, options);

    /// <summary>替换JSON字符串代表的对象</summary>
    public void ReplaceValue(object? obj)
    {
        JsonString = JsonSerializer.Serialize(obj, options);
    }

    void CheckSubType(string jsonString)
    {
        if (SubType == JSONSubType.Array && !jsonString.StartsWith('['))
        {
            throw new ArgumentException("value is not a valid JSONArray");
        }
        if (SubType == JSONSubType.Object && !jsonString.StartsWith('{'))
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