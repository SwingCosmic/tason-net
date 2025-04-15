using TASON.Serialization;

namespace TASON.Types.SystemTextJson;


public class JSONType : TasonScalarTypeBase<JSON>
{
    /// <inheritdoc/>
    protected override JSON DeserializeCore(string text, SerializerOptions options)
    {
        return new JSON(text, options.GetJsonOptions(), JSONSubType.All);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSON value, SerializerOptions options)
    {
        return value.JsonString;
    }
}


public class JSONArrayType : TasonScalarTypeBase<JSON>
{
    /// <inheritdoc/>
    protected override JSON DeserializeCore(string text, SerializerOptions options)
    {
        return new JSON(text, options.GetJsonOptions(), JSONSubType.Array);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSON value, SerializerOptions options)
    {
        return value.JsonString;
    }
}

public class JSONObjectType : TasonScalarTypeBase<JSON>
{
    /// <inheritdoc/>
    protected override JSON DeserializeCore(string text, SerializerOptions options)
    {
        return new JSON(text, options.GetJsonOptions(), JSONSubType.Object);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSON value, SerializerOptions options)
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
#pragma warning restore CS1591
}