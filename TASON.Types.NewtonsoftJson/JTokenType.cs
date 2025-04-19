using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TASON.Serialization;

namespace TASON.Types.NewtonsoftJson;

public class JTokenType : TasonScalarTypeBase<JToken>
{
    /// <inheritdoc/>
    protected override JToken DeserializeCore(string text, TasonSerializerOptions options)
    {
        return JToken.Parse(text, new JsonLoadSettings
        {
            DuplicatePropertyNameHandling = options.AllowDuplicatedKeys 
                ? DuplicatePropertyNameHandling.Replace 
                : DuplicatePropertyNameHandling.Error,
        });
    }

    /// <inheritdoc/>
    protected override string SerializeCore(JToken value, TasonSerializerOptions options)
    {
        return value.ToString();
    }
}
