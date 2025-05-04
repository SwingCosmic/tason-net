# 包TASON.Types.SystemTextJson

该包提供了基于`System.Text.Json`的JSON类型实现，包括`JSON`, `JSONObject`和 `JSONArray`。

支持处理`JsonDocument`, `JsonElement`, `JsonObject`, `JsonArray`类型

## 快速开始

```csharp
using TASON.Types.SystemTextJson;
using System.Text.Json;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
// 通过指定JsonSerializerOptions来配置JSON类型的序列化支持
TasonSerializer.Default.Registry.AddSystemTextJson(options);
```