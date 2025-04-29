
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TASON.Serialization;
using TASON.Util;

namespace TASON.Metadata;

/// <summary>
/// 获取指定类型的TASON属性信息
/// </summary>
public class TasonClassMetadata : ITasonTypeMetadata
{
    internal static void ThrowIfNotValidType(Type type)
    {
        var isValid = type switch
        {
            { IsValueType: true } => true,
            { IsClass: true } => !type.IsAbstract && !typeof(Delegate).IsAssignableFrom(type),
            _ => false,
        };

        if (!isValid)
        {
            throw new InvalidOperationException($"Invalid type '{type.Name}'");
        }
    }

    /// <inheritdoc/>
    public Dictionary<string, PropertyInfo> Properties { get; }
    /// <inheritdoc/>
    public KeyValuePair<string, PropertyInfo>? ExtraFieldsProperty { get; }
    /// <inheritdoc/>
    public Type Type { get; }

    /// <summary>
    /// 根据指定的类型创建新实例
    /// </summary>
    /// <param name="type">要检查的类型</param>
    /// <exception cref="InvalidOperationException">
    /// <list type="bullet">
    ///   <item><see cref="TasonExtraFieldsAttribute"/>出现在多于一个属性上</item>
    ///   <item>类型是抽象类、委托，或者类和结构以外的其它类型</item>
    /// </list>
    /// </exception>
    public TasonClassMetadata(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        ThrowIfNotValidType(type);

        Type = type;
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
                ExtraFieldsProperty = new(realName, p);
            }
            else
            {
                Properties[realName] = p;
            }
        }
    }

}
