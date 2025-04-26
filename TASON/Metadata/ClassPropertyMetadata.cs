
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TASON.Serialization;
using TASON.Util;

namespace TASON.Metadata;

/// <summary>
/// 获取指定类型的TASON属性信息
/// </summary>
internal class ClassPropertyMetadata
{
    /// <summary>类的属性</summary>
    public Dictionary<string, PropertyInfo> Properties { get; }

    public KeyValuePair<string, PropertyInfo>? ExtraFieldsProperty { get; }

    public ClassPropertyMetadata(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        Properties = new();

        var contractAttr = type.GetCustomAttribute<TasonNamingContractAttribute>(true);
        foreach (var p in type.GetProperties())
        {
            if (p.GetCustomAttribute<TasonIgnoreAttribute>(true) is not null)
                continue;

            if (p.GetIndexParameters().Length > 0) 
                continue;

            var realName = p.Name;
            var aliasAttr = p.GetCustomAttribute<TasonPropertyAttribute>(true);
            if (aliasAttr is not null)
                realName = aliasAttr.Name;
            else if (contractAttr is not null)
                realName = p.Name.ToCase(contractAttr.Policy);

            if (p.GetCustomAttribute<TasonExtraFieldsAttribute>(true) is not null)
            {
                if (ExtraFieldsProperty is not null)
                {
                    throw new InvalidOperationException("TasonExtraFieldsAttribute should apply to only one field or property");
                }
                ExtraFieldsProperty = new (realName, p);
            } else
            {
                Properties[realName] = p;
            }    
        }
    }

    /// <summary>
    /// 通过静态泛型类缓存反射结果
    /// </summary>
    /// <typeparamref name="T">缓存的类型</typeparamref>
    public static class Cache<T>
    {
        public static ClassPropertyMetadata Metadata { get; }
        public static Dictionary<string, PropertyInfo> Properties => Metadata.Properties;
        public static KeyValuePair<string, PropertyInfo>? ExtraFieldsProperty => Metadata.ExtraFieldsProperty;

        static Cache()
        {
            Metadata = new ClassPropertyMetadata(typeof(T));
        }
    }
}
