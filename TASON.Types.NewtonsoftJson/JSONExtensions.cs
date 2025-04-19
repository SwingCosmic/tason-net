using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TASON.Types.NewtonsoftJson;

public static class JSONExtensions
{
    private const string LibName = "NewtonsoftJson";
    internal static JsonSerializerSettings DefaultOptions = new();

    /// <summary>
    /// 向<paramref name="registry"/>注册基于Newtonsoft.Json的JSON类型实现
    /// </summary>
    /// <param name="registry">TASON类型注册表</param>
    /// <param name="jsonOptions"><see cref="JsonSerializerSettings"/></param>
    /// <returns></returns>
    public static TasonTypeRegistry AddNewtonsoftJson(this TasonTypeRegistry registry, JsonSerializerSettings? jsonOptions = null)
    {
        jsonOptions ??= new JsonSerializerSettings();
        DefaultOptions = jsonOptions;
        registry.Options.ExtraOptions[LibName] = jsonOptions;

        // 注册默认实现
        registry.RegisterType(nameof(JSONTypes.JSON), JSONTypes.JSON);
        registry.RegisterType(nameof(JSONTypes.JSONArray), JSONTypes.JSONArray);
        registry.RegisterType(nameof(JSONTypes.JSONObject), JSONTypes.JSONObject);

        // 注册Newtonsoft.Json类型为其它实现
        registry.RegisterType(nameof(JSONTypes.JSON), JSONTypes.JToken);

        return registry;
    }

    /// <summary>
    /// 从<paramref name="options"/>中获取<see cref="JsonSerializerSettings"/>
    /// </summary>
    /// <param name="options">已添加<see cref="JsonSerializerSettings"/>的TASON序列化选项，通常通过<see cref="AddNewtonsoftJson" />设置</param>
    public static JsonSerializerSettings GetJsonOptions(this TasonSerializerOptions options)
    {
        if (options.ExtraOptions.TryGetValue(LibName, out var jsonOptions))
        {
            return (JsonSerializerSettings)jsonOptions!;
        }
        Console.WriteLine("[WARN] Cannot find registered JsonSerializerSettings.");
        return DefaultOptions;
    }

}