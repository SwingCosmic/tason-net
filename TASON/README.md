# 包TASON

包含TASON序列化和反序列化的核心逻辑与内置类型实现。


## 反序列化模式

`tason-net`支持两种反序列化模式：自动类型模式和指定类型模式

### 指定类型模式

和其它序列化框架一致，通过指定所需的类型（泛型参数或者Type对象）来反序列化特定类型的实例。该模式下，

* 支持鸭子类型反序列化，即同一名称的TASON类型实例，可以有多种不同的.NET实现类型。例如，TASON`Timestamp`类型实例，可以反序列化为`DateTimeOffset`类型和内置的`Timestamp`类型，取决于你传入的Type是哪个
* 支持将`IEnumerable<T>`和`IDictionary<TKey, TValue>`反序列化为自定义的实现类型
  * `IEnumerable<T>`的实现类需要有一个公共无参构造函数，或者有一个参数传入`IEnumerable<T>`的子类。
  * `IDictionary<TKey, TValue>`的实现类需要有一个公共无参构造函数。


### 自动类型模式

TASON特色功能，通过TASON文本包含的信息自动匹配最合适的.NET类型进行反序列化。该模式下，

* 如果遇到TASON类型实例，会反序列化为该类型的默认.NET实现
* 对于嵌套数组或者对象，会自动反序列化为`List<object>`或者`Dictionary<string, object>`
* 可以使用dynamic变量来接收序列化的对象，从而无视复杂结构快速获取到所需的数据。相比之下，传统序列化框架只提供一些没有类型信息的语法节点，如`System.Text.Json.JsonElement`，遍历和处理此类节点，特别是反序列化深度嵌套的部分节点，以及使用现有.NET对象替换一部分节点十分困难。

自动类型模式尤其适合动态语言如JavaScript生成的复杂动态数据结构，在保留动态结构灵活性的同时，仍然保留强类型的部分。

例如在执行MongoDB查询时，可以避免前端因为序列化为JSON时，Date和Int64等类型变成字符串丢失类型，从而影响查询的执行结果的情况发生。


## 快速开始

```csharp
using TASON;

// 使用默认序列化器
var serializer = TasonSerializer.Default;

// 使用默认实现注册POCO类
serializer.Registry.CreateObjectType(typeof(SomeModelClass));

// 序列化

var model = new SomeModelClass 
{
  SomeProperty = "SomeValue",
  LongValue = 2L << 56,
};
string tason = serializer.Serialize(model); 
// SomeModelClass({SomeProperty:"SomeValue",LongValue:Int64("144115188075855872")})


// 自动类型模式反序列化

var list = serializer.Deserialize($"[{tason}]"); 
// 得到List<object>，其中每个元素都是SomeModelClass

var dict = serializer.Deserialize("{a:1,b:'foo'}"); 
// 得到Dictionary<string, object>，其中每个键值对都是自动推断类型


// 指定类型模式反序列化

var list2 = serializer.Deserialize<ICollection<SomeModelClass>>($"[{tason}]"); 
//得到List<SomeModelClass>

```


## .NET版本差异

⚠️ 由于不同.NET版本之间库API的差异，各个版本提供的功能存在区别，主要有以下方面：

### 基础数字类型

* CLR新增了一些数字类型
  * `Half`: 在不支持的环境中使用了[Half](https://github.com/qingfengxia/System.Half)包，
在支持的.NET版本中，使用原生的`System.Half`，两者公开的API几乎一致。
  * `Int128`和`NFloat`: 仅在受支持的版本中启用
* .NET 7引入的泛型数学，特别是abstract static接口方法的支持，对基础数据类型的支持有较大的变动，例如泛型参数约束等
* 字符串解析的支持不同，特别是非十进制解析，部分未提供的方法采用了可能效率较低的实现

### 日期类型

* .NET 6新增了一些日期类型`DateOnly`和`TimeOnly`：在不受支持的环境中使用了[Portable.System.DateTimeOnly](https://github.com/OlegRa/System.DateTimeOnly)包，在支持的版本中使用原生类型，两者公开的API几乎一致。

## 类型支持

以下列出了序列化和反序列化中.NET类型的支持情况，不含TASON规范禁止的类型
### 完全支持的类型

* 所有基元类型和数字类型，包含`BigInteger`等
* 所有带有无参构造函数的非泛型、非抽象类和结构
* 注册了自定义TASON类型信息的标量类型和对象类型，包括所有内置类型
* 以上类型作为嵌套类型
* 以上类型的数组
* `Nullable<T>`类型

### 完全支持序列化，但反序列化有一定限制的类型

* 完全支持的类型作为泛型参数`T`的集合类型
  * `IEnumerable<T>`、`IDictionary<string, TValue>`及其衍生接口
  * 具有无参构造函数，或者`IEnumerable<T>`类型参数构造函数的`IEnumerable<T>`非抽象实现类，如`List<T>`
  * 具有无参构造函数的`IDictionary<string, TValue>`的非抽象实现类，如`Dictionary<string, TValue>`
* 非泛型集合类
  * 仅支持`ArrayList`、`Queue`、`Stack`
* Key不是字符串的`IDictionary<TKey, TValue>`类型，需要开启参数
* 抽象类和`IEnumerable<T>`以外的接口
  * 仅支持对象类型实例，可以进行多态反序列化

### 可以序列化，暂时无法反序列化的类型

* ValueTuple
* 泛型类
* 匿名类型


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

