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

        // 注册默认实现
        registry.RegisterType(nameof(JSONTypes.JSON), JSONTypes.JSON);
        registry.RegisterType(nameof(JSONTypes.JSONArray), JSONTypes.JSONArray);
        registry.RegisterType(nameof(JSONTypes.JSONObject), JSONTypes.JSONObject);

        // 注册System.Text.Json类型为其它实现
        registry.RegisterType(nameof(JSONTypes.JSON), JSONTypes.JsonDocument);
        registry.RegisterType(nameof(JSONTypes.JSON), JSONTypes.JsonElement);

        return registry;
    }

    /// <summary>
    /// 从<paramref name="options"/>中获取<see cref="JsonSerializerOptions"/>
    /// </summary>
    /// <param name="options">已添加<see cref="JsonSerializerOptions"/>的TASON序列化选项，通常通过<see cref="AddSystemTextJson" />设置</param>
    public static JsonSerializerOptions GetJsonOptions(this TasonSerializerOptions options)
    {
        if (options.ExtraOptions.TryGetValue(LibName, out var jsonOptions))
        {
            return (JsonSerializerOptions)jsonOptions!;
        }
        Console.WriteLine("[WARN] Cannot find registered JsonSerializerOptions.");
        return DefaultOptions;
    }

    /// <summary>
    /// 从<see cref="JsonSerializerOptions"/>获取<see cref="JsonDocumentOptions"/>
    /// </summary>
    /// <param name="options"><see cref="JsonSerializerOptions"/></param>
    public static JsonDocumentOptions GetDocumentOptions(this JsonSerializerOptions options)
    {
        return new JsonDocumentOptions
        {
            AllowTrailingCommas = options.AllowTrailingCommas,
            CommentHandling = options.ReadCommentHandling,
            MaxDepth = options.MaxDepth,
        };
    }
}