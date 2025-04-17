using System.Text.Json;
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
