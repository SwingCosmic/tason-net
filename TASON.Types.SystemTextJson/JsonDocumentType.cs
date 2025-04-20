using System.Text.Json;
using System.Text.Json.Nodes;
using TASON.Serialization;

namespace TASON.Types.SystemTextJson;

public class JsonDocumentType : TasonScalarTypeBase<JsonDocument>
{
    /// <inheritdoc/>
    protected override JsonDocument DeserializeCore(string text, TasonSerializerOptions options)
    {
        return JsonDocument.Parse(text, options.GetJsonOptions().GetDocumentOptions());
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JsonDocument value, TasonSerializerOptions options)
    {
        return value.RootElement.ToString();
    }
}

public class JsonElementType : TasonScalarTypeBase<JsonElement>
{
    /// <inheritdoc/>
    protected override JsonElement DeserializeCore(string text, TasonSerializerOptions options)
    {
        return JsonDocument.Parse(text, options.GetJsonOptions().GetDocumentOptions())
            .RootElement.Clone();
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JsonElement value, TasonSerializerOptions options)
    {
        return value.ToString();
    }
}

public class JsonObjectType : TasonScalarTypeBase<JsonObject>
{
    /// <inheritdoc/>
    protected override JsonObject DeserializeCore(string text, TasonSerializerOptions options)
    {
        var node = JsonNode.Parse(text, documentOptions: options.GetJsonOptions().GetDocumentOptions());
        return node as JsonObject ?? throw new ArgumentException($"Node {node?.GetValueKind()} is not a JSONObject");
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JsonObject value, TasonSerializerOptions options)
    {
        return value.ToString();
    }
}

public class JsonArrayType : TasonScalarTypeBase<JsonArray>
{
    /// <inheritdoc/>
    protected override JsonArray DeserializeCore(string text, TasonSerializerOptions options)
    {
        var node = JsonNode.Parse(text, documentOptions: options.GetJsonOptions().GetDocumentOptions());
        return node as JsonArray ?? throw new ArgumentException($"Node {node?.GetValueKind()} is not a JSONArray");
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JsonArray value, TasonSerializerOptions options)
    {
        return value.ToString();
    }
}
