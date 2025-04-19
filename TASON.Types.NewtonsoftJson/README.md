# 包TASON.Types.NewtonsoftJson

该包提供了基于`JSON.NET`(`Newtonsoft.Json`)的JSON类型实现，包括`JSON`, `JSONObject`和 `JSONArray`。

支持处理`JToken`类型

## 快速开始

```csharp
using TASON.Types.NewtonsoftJson;
using Newtonsoft.Json;

var options = new JsonSerializerSettings
{
  ContractResolver = new CamelCasePropertyNamesContractResolver(),
};
// 通过指定JsonSerializerSettings来配置JSON类型的序列化支持
TasonSerializer.Default.Registry.AddNewtonsoftJson(options);
```