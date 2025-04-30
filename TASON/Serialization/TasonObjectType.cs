
using System.Reflection;
using TASON.Metadata;

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
    public virtual object Deserialize(Dictionary<string, object?> dict, TasonSerializerOptions options)
    {
        return SerializationHelpers.DeserializeClass<T>(dict);
    }

    /// <inheritdoc/>
    public virtual Dictionary<string, object?> Serialize(object value, TasonSerializerOptions options)
    {
        return SerializationHelpers.SerializeType<T>(value);
    }
}
