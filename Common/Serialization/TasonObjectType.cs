
using System.Reflection;
using TASON.Metadata;
using TASON.Util;

namespace TASON.Serialization;


/// <summary>
/// 对象类型实例信息的默认实现。默认实现要求类提供公共无参构造函数，并使用公共读写属性进行序列化。
/// </summary>
public class TasonObjectType<T> : ITasonObjectType where T : notnull, new()
{
    /// <inheritdoc/>
    public TasonTypeInstanceKind Kind { get; }
    /// <inheritdoc/>
    public Type Type { get; }

    public TasonObjectType()
    {
        Type = typeof(T);
        Kind = TasonTypeInstanceKind.Object;
    }

    /// <inheritdoc/>
    public virtual object Deserialize(Dictionary<string, object?> dict, SerializerOptions options)
    {
        var obj = new T();
        foreach (var (name, prop) in ClassPropertyMetadata.Cache<T>.Properties)
        {
            if (dict.TryGetValue(name, out var value))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                }
            }
        }
        return obj;
    }

    /// <inheritdoc/>
    public virtual Dictionary<string, object?> Serialize(object value, SerializerOptions options)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var (name, prop) in ClassPropertyMetadata.Cache<T>.Properties)
        {
            dict[name] = prop.GetValue(value);
        }
        return dict;
    }
}