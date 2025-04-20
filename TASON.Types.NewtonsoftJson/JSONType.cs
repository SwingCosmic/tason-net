using TASON.Serialization;

namespace TASON.Types.NewtonsoftJson;


public class JSONType : TasonScalarTypeBase<JSONNewtonsoft>
{
    /// <inheritdoc/>
    protected override JSONNewtonsoft DeserializeCore(string text, TasonSerializerOptions options)
    {
        return new JSONNewtonsoft(text, options.GetJsonOptions(), JSONSubType.All);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSONNewtonsoft value, TasonSerializerOptions options)
    {
        return value.JsonString;
    }
}


public class JSONArrayType : TasonScalarTypeBase<JSONNewtonsoft>
{
    /// <inheritdoc/>
    protected override JSONNewtonsoft DeserializeCore(string text, TasonSerializerOptions options)
    {
        return new JSONNewtonsoft(text, options.GetJsonOptions(), JSONSubType.Array);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSONNewtonsoft value, TasonSerializerOptions options)
    {
        return value.JsonString;
    }
}

public class JSONObjectType : TasonScalarTypeBase<JSONNewtonsoft>
{
    /// <inheritdoc/>
    protected override JSONNewtonsoft DeserializeCore(string text, TasonSerializerOptions options)
    {
        return new JSONNewtonsoft(text, options.GetJsonOptions(), JSONSubType.Object);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JSONNewtonsoft value, TasonSerializerOptions options)
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


    public static JTokenType JToken { get; } = new();
    public static JObjectType JObject { get; } = new();
    public static JArrayType JArray { get; } = new();
#pragma warning restore CS1591
}