
using System.Reflection;
using TASON.Util;

namespace TASON.Serialization;

/// <summary>
/// 获取指定类型的TASON属性信息
/// </summary>
internal class ObjectTypePropertyInfo
{
    /// <summary>类的属性</summary>
    public Dictionary<string, PropertyInfo> Properties { get; }

    public ObjectTypePropertyInfo(Type type)
    {
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
