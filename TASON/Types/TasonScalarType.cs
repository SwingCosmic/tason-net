
using System.Reflection;

namespace TASON.Types;

/// <summary>
/// 标量类型实例信息的默认实现。默认实现要求类带有一个string参数的构造函数，
/// 并且在<see cref="object.ToString"/>方法中返回序列化后的值
/// </summary>
public class TasonScalarType<T> : ITasonScalarType where T : notnull, IEquatable<T>
{
    /// <inheritdoc/>
    public TasonTypeInstanceKind Kind { get; }
    /// <inheritdoc/>
    public Type Type { get; }

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

    public TasonScalarType()
    {
        Type = typeof(T);
        Kind = TasonTypeInstanceKind.Scalar;
    }

    /// <inheritdoc/>
    public virtual object Deserialize(string text, SerializerOptions options)
    {
        return ReflectionCache.Ctor.Invoke([text]);
    }

    /// <inheritdoc/>
    public virtual string Serialize(object value, SerializerOptions options)
    {
        return value.ToString() ?? "";
    }
}
