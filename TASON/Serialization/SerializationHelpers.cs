using System.Reflection;
using TASON.Metadata;
using TASON.Util;
using KV = System.Collections.Generic.Dictionary<string, object?>;

namespace TASON.Serialization;

internal static class SerializationHelpers
{


    /// <summary>
    /// 获取指定属性TASON序列化时所用的名称
    /// </summary>
    /// <param name="property">要获取的属性</param>
    /// <param name="namingContract">可选，属性所在的类的<see cref="TasonNamingContractAttribute" /></param>
    /// <returns></returns>
    public static string GetPropertyName(PropertyInfo property, TasonNamingContractAttribute? namingContract = null)
    {
        var realName = property.Name;
        var aliasAttr = property.GetCustomAttribute<TasonPropertyAttribute>(true);
        if (aliasAttr is not null)
            realName = aliasAttr.Name;
        else if (namingContract is not null)
            realName = property.Name.ToCase(namingContract.Policy);

        return realName;
    }

    /// <summary>
    /// 获取指定字段TASON序列化时所用的名称
    /// </summary>
    /// <param name="field">要获取的字段</param>
    /// <param name="namingContract">可选，字段所在的类的<see cref="TasonNamingContractAttribute" /></param>
    /// <returns></returns>
    public static string GetFieldName(FieldInfo field, TasonNamingContractAttribute? namingContract = null)
    {
        var realName = field.Name;
        var aliasAttr = field.GetCustomAttribute<TasonPropertyAttribute>(true);
        if (aliasAttr is not null)
            realName = aliasAttr.Name;
        else if (namingContract is not null)
            realName = field.Name.ToCase(namingContract.Policy);

        return realName;
    }


    public static T DeserializeClass<T>(KV dict) where T : notnull, new()
    {
        // 不在泛型约束上限制class，不然无法通过继承TasonObjectType自定义反序列化结构
        if (typeof(T).IsValueType) 
            throw new NotSupportedException($"Cannot deserialize value type '{typeof(T).Name}' through reflection");

        var obj = new T();
        var meta = TasonTypeMetadataProvider.GetMetadata<T>();
        foreach (var (name, prop) in meta.Properties)
        {
            if (dict.Remove(name, out var value))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                }
            }
        }
        var extra = meta.ExtraMemberProperty;
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
    

    public static KV SerializeType<T>(object value) where T : notnull
    {
        var dict = new KV();
        var meta = TasonTypeMetadataProvider.GetMetadata<T>();
        foreach (var (name, prop) in meta.Properties)
        {
            dict[name] = prop.GetValue(value);
        }
        var extra = meta.ExtraMemberProperty;
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