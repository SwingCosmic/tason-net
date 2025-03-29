using System.Collections.Generic;

namespace TASON;

/// <summary>
/// 序列化选项
/// </summary>
public class SerializerOptions
{
    /// <summary>
    /// 是否允许使用不安全的类型，默认<see langword="false"/>
    /// </summary>
    public bool AllowUnsafeTypes { get; set; } = false;

    /// <summary>
    /// 是否使用内置字典类型来序列化<see cref="IDictionary{K,V}"/>，默认<see langword="false"/>（序列化为普通对象）
    /// </summary>
    public bool UseBuiltinDictionary { get; set; } = false;

    /// <summary>
    /// 是否允许对象拥有重复的键，默认<see langword="true"/>
    /// </summary>
    public bool AllowDuplicatedKeys { get; set; } = true;

    /// <summary>
    /// 序列化时的缩进大小（单位为空格数），0表示不缩进，<see langword="null"/>表示压缩内容。默认<see langword="null"/>
    /// </summary>
    public int? Indent { get; set; } = null;

    /// <summary>
    /// 最大递归深度，默认64
    /// </summary>
    public int MaxDepth { get; set; } = 64;

    /// <summary>
    /// 序列化时如何使用内置数值类型
    /// </summary>
    public BuiltinNumberOption UseBuiltinNumber { get; set; } = BuiltinNumberOption.UnsafeOnly;


    /// <summary>
    /// 由其他库提供的额外选项
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