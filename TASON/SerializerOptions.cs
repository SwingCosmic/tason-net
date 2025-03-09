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
    /// 反序列化对象时是否采用`Object.create(null)`，默认<see langword="false"/>
    /// </summary>
    public bool NullPrototypeObject { get; set; } = false;

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
}