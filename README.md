# tason-net

![nuget](https://img.shields.io/nuget/v/TASON?label=TASON)
![nuget](https://img.shields.io/nuget/v/TASON.Types.SystemTextJson?label=TASON.Types.SystemTextJson)


[TASON](https://github.com/SwingCosmic/tason)的.NET/C#实现

默认提供`.NET Standard 2.1`(Unity), `.NET 6`和`.NET 8`的nuget包，对于高于`netstandard2.1`的其它.NET版本，可以自行编译。

## Road Map

### v1.x

* 完成TASON规范的功能实现，包括所有内置类型的序列化和反序列化
* 完成常见类型的指定Type反序列化，可满足大多数WebAPI接口的序列化需求
* 提供ASP.NET Core集成支持

### v2.x

* 重构实现，提升序列化性能
* 增加源生成器，提供AOT支持以针对非Web场景的支持

### 参与开发说明

⚠️ 由于TASON的目标是强类型的动态类型序列化，更加注重语义描述而不是性能，
因此在准备进入2.0版本之前，请不要提前针对实现细节进行过度性能优化和AOT支持，这可能导致兼容性出现问题。包括但不限于：

* 重新实现高性能解析器
* 大面积使用无额外内存分配的类型如`ReadOnlySpan<T>`替换公共API
* 限制可序列化类型种类，例如需要用专用Attribute来标记支持的类型
* 提供一些配置以用于少见的序列化需求，例如序列化私有字段，这些可以通过自定义TASON类型信息实现
* 增加难以跨语言支持的特性，例如泛型类型等

## .NET版本差异

⚠️ 由于不同.NET版本之间库API的差异，各个版本提供的功能存在区别，主要有以下方面：

### 基础数字类型

* CLR新增了一些数字类型
  * `Half`: 在不支持的环境中使用了[Half](https://github.com/qingfengxia/System.Half)包，
在支持的.NET版本中，使用原生的`System.Half`，两者公开的API几乎一致。
  * `Int128`和`NFloat`: 仅在受支持的版本中启用
* .NET 7引入的泛型数学，特别是abstract static接口方法的支持，对基础数据类型的支持有较大的变动，例如泛型参数约束等
* 字符串解析的支持不同，特别是非十进制解析，部分未提供的方法采用了可能效率较低的实现


## 内置类型支持

相比JavaScript版本，针对语言特性提供了更多的内置类型支持

### 整数和浮点数类型

标✅是TASON默认支持类型，标✨是.NET特有类型

⚠️ TASON将`IntPtr`和`UIntPtr`视为本机大小的整数，即C#关键字`nint`和`nuint`对应的CLR类型，而不是指针的包装类型。
在序列化时会使用对应的普通整数类型进行序列化，因此可以避免一些可能的安全问题。
但为了避免在代码中无意识地转成指针类型，仍然需要开启`AllowUnsafeTypes`选项。

|名称|描述|别名<br />(CLR类型名/关键字)|备注|
|-|-|-|-|
|`Int8`|8位有符号整数|`SByte`||
|✅`UInt8`|8位无符号整数|`Byte`||
|✅`Int16`|16位有符号整数|`Short`||
|`UInt16`|16位无符号整数|||
|✨`Char`|16位字符||作为UInt16处理|
|✅`Int32`|32位有符号整数|`Int`||
|`UInt32`|32位无符号整数|||
|✅`Int64`|64位有符号整数|`Long`|
|`UInt64`|64位无符号整数|||
|`Int128`|128位有符号整数||.NET 7+|
|`UInt128`|128位无符号整数||.NET 7+|
|✅`BigInt`|无限精度整数|`BigInteger`||
|`Float16`|16位浮点数|`Half`|.NET 5+，带有补丁支持|
|✅`Float32`|32位浮点数|`Single`||
|✅`Float64`|64位浮点数|`Double`||
|✅`Decimal128`|128位有符号十进制数|`Decimal`||
|✨`NFloat`|本机大小浮点数||.NET 6+，作为Float32或Float64处理|
|✨`IntPtr`|本机大小有符号整数||作为Int32或Int64处理|
|✨`UIntPtr`|本机大小无符号整数||作为UInt32或UInt64处理|

