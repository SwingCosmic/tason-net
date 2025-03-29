using System.Text.Json;

namespace TASON.Types.SystemTextJson;

public static class JSONExtensions
{
    private const string LibName = "SystemTextJson";
    internal static JsonSerializerOptions DefaultOptions = JsonSerializerOptions.Default;

    /// <summary>
    /// 向<paramref name="registry"/>注册基于System.Text.Json的JSON类型实现
    /// </summary>
    /// <param name="registry">TASON类型注册表</param>
    /// <param name="jsonOptions"><see cref="JsonSerializerOptions"/></param>
    /// <returns></returns>
    public static TasonTypeRegistry AddSystemTextJson(this TasonTypeRegistry registry, JsonSerializerOptions? jsonOptions = null)
    {
        jsonOptions ??= JsonSerializerOptions.Default;
        DefaultOptions = jsonOptions;
        registry.Options.ExtraOptions[LibName] = jsonOptions;

        registry.RegisterType(nameof(JSONTypes.JSON), JSONTypes.JSON);
        registry.RegisterType(nameof(JSONTypes.JSONArray), JSONTypes.JSONArray);
        registry.RegisterType(nameof(JSONTypes.JSONObject), JSONTypes.JSONObject);

        return registry;
    }

    public static JsonSerializerOptions GetJsonOptions(this SerializerOptions options)
    {
        if (options.ExtraOptions.TryGetValue(LibName, out var jsonOptions))
        {
            return (JsonSerializerOptions)jsonOptions!;
        }
        Console.WriteLine("[WARN] Cannot find registered JsonSerializerOptions.");
        return DefaultOptions;
    }
}