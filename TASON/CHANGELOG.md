# 1.0.0-rc.1

* feat: 自定义类型元数据，针对现存的采用私有字段存值的类型

# 0.14.2

* fix: 注册类型实例时没有处理字段的序列化

# 0.14.1

* feat: 新增`AllowFields`序列化选项，现在可以序列化和反序列化公共字段
* ⚠️ BREAKING CHANGE: 将`TasonExtraFieldsAttribute`重命名为`TasonExtraMemberAttribute`，
以避免"field"这个称呼造成混淆，实际上包含任何不在对象的公共属性和字段定义中的键值对

# 0.13.2

* fix: 现在可以反序列化结构了，通过注册自定义对象类型实例
* refact: 重构了元数据相关的类型和实现

# 0.13.1

* feat: 新增`NullPropertyHandling`序列化选项
* refact: 现在`TasonWriter`是抽象类，分别由具体的子类实现关于回溯的逻辑
* fix: 修复`TasonWriter`处理属性跳过时最后一个尾随逗号的处理
* ⚠️ BREAKING CHANGE: 将`UseBuiltinNumber`选项重命名为`BuiltinNumberHandling`

# 0.12.3

* feat: 新增`DateOnly`和`TimeOnly`类型支持
* refact: 重新拆分和移动单元测试相关代码，更加符合逻辑

# 0.12.2

* fix: 多态反序列化报错，无法将具体的TASON类型实例反序列化给其抽象基类或者接口

# 0.12.1

* feat: 新增`TasonExtraFieldsAttribute`，效果类似于`JsonExtensionDataAttribute`，用于扩展字段的序列化与反序列化。
同时支持普通对象和TASON对象实例

# 0.11.4

* feat: 支持`Nullable<T>`类型

# 0.11.3

* 增加更多JSON类型支持，如`Newtonsoft.Json`的`JObject`
* fix: 类存在索引器会序列化报错

# 0.11.2

* 重构JSON类型支持，将`JSON`提供为抽象类，以同时支持`System.Text.Json`和`Newtonsoft.Json`

# 0.11.0

* ⚠️ BREAKING CHANGE: 将类`SerializerOptions`重命名为`TasonSerializerOptions`，以减少类型名冲突