# 0.11.4

* feat: 支持`Nullable<T>`类型

# 0.11.3

* 增加更多JSON类型支持，如`Newtonsoft.Json`的`JObject`
* fix: 类存在索引器会序列化报错

# 0.11.2

* 重构JSON类型支持，将`JSON`提供为抽象类，以同时支持`System.Text.Json`和`Newtonsoft.Json`

# 0.11.0

* ⚠️ BREAKING CHANGE: 将类`SerializerOptions`重命名为`TasonSerializerOptions`，以减少类型名冲突