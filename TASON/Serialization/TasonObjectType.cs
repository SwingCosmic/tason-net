
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
    public virtual object Deserialize(Dictionary<string, object?> dict, TasonSerializerOptions options)
    {
        var obj = new T();
        foreach (var (name, prop) in ClassPropertyMetadata.Cache<T>.Properties)
        {
            if (dict.Remove(name, out var value))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                }
            }
        }
        var extra = ClassPropertyMetadata.Cache<T>.ExtraFieldsProperty;
        if (extra is not null)
        {
            var fieldsDict = ReflectionHelpers.CreateDictionary<string, object?>(extra.Value.Value.PropertyType);
            foreach (var (k, v) in dict)
            {
                fieldsDict[k] = v;
            }
            extra.Value.Value.SetValue(obj, fieldsDict);
        }
        return obj;
    }

    /// <inheritdoc/>
    public virtual Dictionary<string, object?> Serialize(object value, TasonSerializerOptions options)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var (name, prop) in ClassPropertyMetadata.Cache<T>.Properties)
        {
            dict[name] = prop.GetValue(value);
        }
        var extra = ClassPropertyMetadata.Cache<T>.ExtraFieldsProperty;
        if (extra is not null)
        {
            if (extra.Value.Value.GetValue(value) is IDictionary<string, object?> data)
            {
                foreach (var (k, v) in data)
                {
                    dict[k] = v;
                }
            }
        }
        return dict;
    }
}