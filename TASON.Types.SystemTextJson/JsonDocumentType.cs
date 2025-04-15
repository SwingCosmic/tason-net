using System.Text.Json;
using TASON.Serialization;

namespace TASON.Types.SystemTextJson;

public class JsonDocumentType : TasonScalarTypeBase<JsonDocument>
{
    /// <inheritdoc/>
    protected override JsonDocument DeserializeCore(string text, SerializerOptions options)
    {
        return JsonDocument.Parse(text, options.GetJsonOptions().GetDocumentOptions());
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JsonDocument value, SerializerOptions options)
    {
        return value.RootElement.ToString();
    }
}

public class JsonElementType : TasonScalarTypeBase<JsonElement>
{
    /// <inheritdoc/>
    protected override JsonElement DeserializeCore(string text, SerializerOptions options)
    {
        return JsonDocument.Parse(text, options.GetJsonOptions().GetDocumentOptions())
            .RootElement.Clone();
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JsonElement value, SerializerOptions options)
    {
        return value.ToString();
    }
}
