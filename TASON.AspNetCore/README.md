
# 包TASON.AspNetCore

该包提供了对ASP.NET Core的集成支持，包括MVC的模型绑定和响应序列化。

TASON的MIME类型为`application/x-tason`。配置好输入输出Formatters后，

* 将`Accept`头设置为`application/x-tason`，即可使用TASON进行模型绑定。
* 将`Content-Type`头设置为`application/x-tason`，即可使用TASON进行响应序列化。

## 快速开始

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
  // 添加TASON支持
  .AddTason(options =>
  {
    // 配置SerializerOptions
    options.SerializerOptions.Indent = 2;
    // 向TypeRegistry注册额外的类型或者别名
    options.TypeRegistry.RegisterTypeAlias("MyTypeAlias", "MyType");
    // 配置自动注册的类型，例如返回所有的POCO类，要求这些类型有公共无参构造函数
    options.GetAutoRegisterObjectTypes = () =>
    {
      return Assembly
        .GetExecutingAssembly() 
        .GetExportedTypes()
        .Where(t => t.Name.EndsWith("Model"))
        .ToArray();
    }
  });

```