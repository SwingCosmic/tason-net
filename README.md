# tason-net

![nuget](https://img.shields.io/nuget/v/TASON?label=TASON)

![nuget](https://img.shields.io/nuget/v/TASON.Types.SystemTextJson?label=TASON.Types.SystemTextJson)

![nuget](https://img.shields.io/nuget/v/TASON.Types.NewtonsoftJson?label=TASON.Types.NewtonsoftJson)

![nuget](https://img.shields.io/nuget/v/TASON.AspNetCore?label=TASON.AspNetCore)


[TASON](https://github.com/SwingCosmic/tason)的.NET/C#实现

默认提供`.NET Standard 2.1`(Unity), `.NET 6`和`.NET 8`的nuget包，对于高于`netstandard2.1`的其它.NET版本，可以自行编译。

## Road Map

从1.0开始，TASON采用奇偶版本交替的方式发布主版本，偶数版本为稳定版本，奇数版本为开发版本。

每当奇数版本发布后，主分支将一直保持最新开发状态。
功能稳定后，该版本将创建一个偶数版本分支进入LTS并进入维护阶段，该阶段只修复bug和处理安全漏洞，同时主分支进入下一个奇数版本。

### v1

* 完成TASON规范的功能实现，包括所有内置类型的序列化和反序列化
* 完成常见类型的指定Type反序列化，可满足大多数WebAPI接口的序列化需求
* 提供ASP.NET Core集成支持
* 与更多现有类型和库的集成，如MongoDB和GraphQL等

### v3

* 重构实现，提升序列化性能
* 增加源生成器，提供AOT支持以针对非Web场景的支持
* 增加泛型支持

### 参与开发说明

⚠️ 由于TASON的目标是强类型的动态类型序列化，更加注重语义描述而不是性能，
因此在准备进入2.0版本之前，请不要提前针对实现细节进行过度性能优化和AOT支持，这可能导致兼容性出现问题。包括但不限于：

* 重新实现高性能解析器
* 大面积使用无额外内存分配的类型如`ReadOnlySpan<T>`替换公共API
* 限制可序列化类型种类，例如需要用专用Attribute来标记支持的类型
* 提供一些配置以用于少见的序列化需求，例如序列化私有字段，这些可以通过自定义TASON类型信息实现
* 增加难以跨语言支持的特性，例如泛型类型等


## 各个包的文档

* [TASON](./TASON/README.md) : TASON的.NET/C#实现
* [TASON.Types.SystemTextJson](./TASON.Types.SystemTextJson/README.md) : 基于`System.Text.Json`的JSON类型实现
* [TASON.Types.NewtonsoftJson](./TASON.Types.NewtonsoftJson/README.md) : 基于`Newtonsoft.Json`的JSON类型实现
* [TASON.AspNetCore](./TASON.AspNetCore/README.md) : ASP.NET Core集成支持，包括MVC的模型绑定和响应序列化

