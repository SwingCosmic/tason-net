
namespace TASON.Serialization;

/// <summary>
/// 指示该字段或者属性需要忽略
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class TasonIgnoreAttribute : Attribute
{

}

/// <summary>
/// 指示该字段或者属性用来存放额外的字段。
/// 类型必须是<see cref="IDictionary{String, Object}"/>的子类型，每个类最多只能有一个包含该特性的属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class TasonExtraMemberAttribute : Attribute
{

}

/// <summary>
/// 指示该字段或者属性序列化所使用的名称
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class TasonPropertyAttribute : Attribute
{
    public TasonPropertyAttribute(string name)
    {
        Name = name;
    }
    /// <summary>序列化所用的名称</summary>
    public string Name { get; }
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
public class TasonNamingContractAttribute : Attribute
{
    public TasonNamingContractAttribute(TasonNamingPolicy policy)
    {
        Policy = policy;
    }

    /// <summary>命名策略</summary>
    public TasonNamingPolicy Policy { get; }
}