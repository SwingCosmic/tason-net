
using System.Reflection;

namespace TASON.Serialization;

/// <summary>
/// <see cref="ITasonScalarType"/>实现的基类，提供了一个泛型版本方便实现
/// </summary>
/// <typeparam name="T">CLR类型</typeparam>
public abstract class TasonScalarTypeBase<T> : ITasonScalarType where T : notnull
{
    /// <inheritdoc/>
    public TasonTypeInstanceKind Kind { get; }
    /// <inheritdoc/>
    public Type Type { get; }

    public TasonScalarTypeBase()
    {
        Type = typeof(T);
        Kind = TasonTypeInstanceKind.Scalar;
    }

    /// <inheritdoc/>
    public object Deserialize(string text, SerializerOptions options)
    {
        return DeserializeCore(text, options);
    }

    /// <inheritdoc/>
    public string Serialize(object value, SerializerOptions options)
    {
        return SerializeCore((T)value, options);
    }

    /// <summary>
    /// 泛型版本的反序列化方法
    /// </summary>
    /// <param name="text">要序列化的标量类型实例</param>
    /// <param name="options">选项</param>
    /// <returns></returns>
    protected abstract T DeserializeCore(string text, SerializerOptions options);
    /// <summary>
    /// 泛型版本的序列化方法
    /// </summary>
    /// <param name="value">代表该类型的字符串</param>
    /// <param name="options">选项</param>
    /// <returns></returns>
    protected abstract string SerializeCore(T value, SerializerOptions options);
}

/// <summary>
/// 标量类型实例信息的默认实现。默认实现要求类带有一个string参数的构造函数，
/// 并且在<see cref="object.ToString"/>方法中返回序列化后的值
/// </summary>
public sealed class TasonScalarType<T> : TasonScalarTypeBase<T> where T : notnull, IEquatable<T>
{
    /// <summary>
    /// 通过捕获外层的泛型使用静态属性缓存反射结果
    /// </summary>
    internal static class ReflectionCache
    {
        /// <summary>类的构造函数</summary>
        public static ConstructorInfo Ctor { get; }

        static ReflectionCache()
        {
            var type = typeof(T);
            Ctor = type.GetConstructor([typeof(string)])!;
            if (Ctor is null)
            {
                throw new ArgumentException($"'{type.Name}' must have a constructor that takes a string");
            }
        }
    }


    /// <inheritdoc/>
    protected override T DeserializeCore(string text, SerializerOptions options)
    {
        return (T)ReflectionCache.Ctor.Invoke([text]);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(T value, SerializerOptions options)
    {
        return value.ToString() ?? "";
    }
}
