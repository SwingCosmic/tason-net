
namespace TASON.Serialization;

/// <summary>
/// 指示该字段或者属性需要忽略
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class TasonIgnoreAttribute : Attribute
{

}

/// <summary>
/// 指示该属性用来存放额外的对象属性。
/// 类型必须是<c>IDictionary&lt;string, object?&gt;</c>的子类型，每个类最多只能有一个包含该特性的属性；
/// 不能放置在字段上
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class TasonExtraMemberAttribute : Attribute
{

}

/// <summary>
/// 指示该字段或者属性序列化所使用的名称
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class TasonPropertyAttribute(string name) : Attribute
{
    /// <summary>序列化所用的名称</summary>
    public string Name { get; } = name;
}

/// <summary>指定属性命名策略</summary>
public enum TasonNamingPolicy
{
    /// <summary>保持原样</summary>
    Preserve = 0,
    /// <summary>camelCase</summary>
    CamelCase,
    /// <summary>PascalCase</summary>
    PascalCase,
    /// <summary>snake_case</summary>
    SnakeCase,
    /// <summary>kebab-case</summary>
    KebabCase,
}

/// <summary>
/// 指定整个类的属性命名约定
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class TasonNamingContractAttribute(TasonNamingPolicy policy) : Attribute
{

    /// <summary>命名策略</summary>
    public TasonNamingPolicy Policy { get; } = policy;
}


/// <summary>
/// 指示该枚举采用字符串值进行序列化
/// </summary>
[AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
public sealed class TasonStringEnumAttribute() : Attribute
{
}

/// <summary>
/// 针对<see cref="TasonStringEnumAttribute"/>标记的枚举，指定枚举值对应的字符串形式
/// </summary>
/// <param name="value"></param>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class TasonEnumValueAttribute(string value) : Attribute
{
    /// <summary>
    /// 序列化的字符串值
    /// </summary>
    public string Value { get; } = value;
}
