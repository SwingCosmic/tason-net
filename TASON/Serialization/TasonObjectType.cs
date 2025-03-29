
using System.Reflection;
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

    /// <summary>
    /// 通过捕获外层的泛型使用静态属性缓存反射结果
    /// </summary>
    internal static class ReflectionCache
    {
        /// <summary>类的属性</summary>
        public static Dictionary<string, PropertyInfo> Properties { get; }

        static ReflectionCache()
        {
            var type = typeof(T);
            Properties = new();

            var contractAttr = type.GetCustomAttribute<TasonNamingContractAttribute>(true);
            foreach (var p in type.GetProperties())
            {
                if (p.GetCustomAttribute<TasonIgnoreAttribute>(true) is not null)
                    continue;

                var realName = p.Name;
                var aliasAttr = p.GetCustomAttribute<TasonPropertyAttribute>(true);
                if (aliasAttr is not null)
                    realName = aliasAttr.Name;
                else if (contractAttr is not null)
                    realName = p.Name.ToCase(contractAttr.Policy);

                Properties[realName] = p;
            }
        }
    }



    public TasonObjectType()
    {
        Type = typeof(T);
        Kind = TasonTypeInstanceKind.Object;
    }

    /// <inheritdoc/>
    public virtual object Deserialize(Dictionary<string, object?> dict, SerializerOptions options)
    {
        var obj = new T();
        foreach (var (name, prop) in ReflectionCache.Properties)
        {
            if (dict.TryGetValue(name, out var value))
            {
                prop.SetValue(obj, value);
            }
        }
        return obj;
    }

    /// <inheritdoc/>
    public virtual Dictionary<string, object?> Serialize(object value, SerializerOptions options)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var (name, prop) in ReflectionCache.Properties)
        {
            dict[name] = prop.GetValue(value);
        }
        return dict;
    }
}