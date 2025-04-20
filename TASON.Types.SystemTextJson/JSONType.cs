using TASON.Serialization;

namespace TASON.Types.SystemTextJson;


public class JSONType : TasonScalarTypeBase<JSONSystemText>
{
    /// <inheritdoc/>
    protected override JSONSystemText DeserializeCore(string text, TasonSerializerOptions options)
    {
        return new JSONSystemText(text, options.GetJsonOptions(), JSONSubType.All);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSONSystemText value, TasonSerializerOptions options)
    {
        return value.JsonString;
    }
}


public class JSONArrayType : TasonScalarTypeBase<JSONSystemText>
{
    /// <inheritdoc/>
    protected override JSONSystemText DeserializeCore(string text, TasonSerializerOptions options)
    {
        return new JSONSystemText(text, options.GetJsonOptions(), JSONSubType.Array);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSONSystemText value, TasonSerializerOptions options)
    {
        return value.JsonString;
    }
}

public class JSONObjectType : TasonScalarTypeBase<JSONSystemText>
{
    /// <inheritdoc/>
    protected override JSONSystemText DeserializeCore(string text, TasonSerializerOptions options)
    {
        return new JSONSystemText(text, options.GetJsonOptions(), JSONSubType.Object);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSONSystemText value, TasonSerializerOptions options)
    {
        return value.JsonString;
    }
}



public static class JSONTypes 
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static JSONType JSON { get; } = new();
    public static JSONArrayType JSONArray { get; } = new();
    public static JSONObjectType JSONObject { get; } = new();


    public static JsonDocumentType JsonDocument { get; } = new();
    public static JsonElementType JsonElement { get; } = new();
    public static JsonObjectType JsonObject { get; } = new();
    public static JsonArrayType JsonArray { get; } = new();
#pragma warning restore CS1591
}