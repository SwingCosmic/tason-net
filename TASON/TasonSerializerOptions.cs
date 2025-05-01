using System.Collections.Generic;

namespace TASON;

/// <summary>
/// 序列化选项
/// </summary>
public record class TasonSerializerOptions
{
    /// <summary>
    /// 是否允许使用不安全的类型，默认<see langword="false"/>
    /// </summary>
    public bool AllowUnsafeTypes { get; set; } = false;

    /// <summary>
    /// 是否使用内置字典类型来序列化<see cref="IDictionary{K,V}"/>，默认<see langword="false"/>（序列化为普通对象，只允许字符串键）
    /// </summary>
    public bool UseBuiltinDictionary { get; set; } = false;

    /// <summary>
    /// 是否允许对象拥有重复的键，默认<see langword="false"/>
    /// </summary>
    public bool AllowDuplicatedKeys { get; set; } = false;

    /// <summary>
    /// 是否允许序列化字段，默认<see langword="false"/>
    /// </summary>
    public bool AllowFields { get; set; } = false;

    /// <summary>
    /// 序列化时的缩进大小，默认<see langword="null"/>
    /// <list type="bullet">
    ///   <item><see langword="null"/>表示尽可能压缩内容，移除不必要的空白字符</item>
    ///   <item>正数值表示启用格式化文档，并设定每一级缩进使用的空格数</item>
    ///   <item>0表示仍然启用格式化文档，但不缩进</item>
    /// </list>
    /// </summary>
    public int? Indent { get; set; } = null;

    /// <summary>
    /// 最大递归深度，默认64
    /// </summary>
    public int MaxDepth { get; set; } = 64;

    /// <summary>
    /// 序列化时如何使用内置数值类型
    /// </summary>
    public BuiltinNumberOption BuiltinNumberHandling { get; set; } = BuiltinNumberOption.UnsafeOnly;

    /// <summary>
    /// 遇到<see langword="null"/>值属性时的序列化方式
    /// </summary>
    public NullValueHandling NullPropertyHandling { get; set; } = NullValueHandling.Preserve;

    /// <summary>
    /// 存储由其他库提供的额外选项
    /// </summary>
    public Dictionary<string, object?> ExtraOptions { get; set; } = new();

}

/// <summary>
/// 指示序列化时如何使用内置数值类型
/// </summary>
public enum BuiltinNumberOption
{
    /// <summary>
    /// 默认值，仅当数值有可能超过<see langword="double"/>所能安全表示的范围时，才使用内置类型。
    /// 对<see langword="long"/>, <see langword="decimal"/>和<see cref="System.Numerics.BigInteger"/>生效。
    /// </summary>
    UnsafeOnly,
    /// <summary>始终使用数字字面量，可能导致精度丢失和溢出</summary>    
    None,
    /// <summary>始终使用内置数值类型，会大幅增加序列化生成的字符串长度</summary>
    All,
    /// <summary>在<see cref="ITasonObjectType"/>的属性中相当于<see cref="All"/>，
    /// 在其他地方相当于<see cref="UnsafeOnly"/>
    /// </summary>
    ObjectTypeProperty,
}

/// <summary>
/// 指示序列化时如何处理<see langword="null"/>
/// </summary>
public enum NullValueHandling
{
    /// <summary>
    /// 默认值，当值为<see langword="null"/>时，序列化为<see langword="null"/>
    /// </summary>
    Preserve,
    /// <summary>
    /// 当值为<see langword="null"/>时，丢弃该属性。
    /// </summary>
    Ignore,
}