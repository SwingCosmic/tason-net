using System.Xml.Linq;

namespace TASON;

internal record class TasonRegistryEntry(string Name, List<ITasonTypeInfo> Types) {

}

/// <summary>
/// TASON类型注册表。大多数情况下，不应该直接创建全新的注册表，
/// 而是从实例的<see cref="TasonSerializer.Registry"/>属性进行克隆
/// </summary>
/// <param name="options">选项</param>
public class TasonTypeRegistry(SerializerOptions options)
{
    Dictionary<string, TasonRegistryEntry> types = new();

    /// <summary>使用当前实例的选项与注册类型创建副本，以进行独立的操作</summary>
    public TasonTypeRegistry Clone()
    {
        return new TasonTypeRegistry(options)
        {
            types = new(types)
        };
    }

    /// <summary>注册一个类型</summary>
    public void RegisterType(string name, ITasonTypeInfo typeInfo)
    {
        var entry = GetEntry(name);
        entry.Types.Add(typeInfo);
    }

    /// <summary>
    /// 注册一个类型别名，指向已有的类型
    /// </summary>
    /// <param name="name">别名</param>
    /// <param name="originName">原有类型名称</param>
    /// <exception cref="InvalidOperationException">原有的类型未注册</exception>
    public void RegisterTypeAlias(string name, string originName) 
    {
        if (!types.TryGetValue(name, out var entry))
        {
            throw new InvalidOperationException($"Type '{originName}' does not exist");
        };
        types[name] = entry;
    }


    #region 获取类型信息

    /// <summary>
    /// 根据指定类型获取<see cref="ITasonTypeInfo"/>，若未注册返回<see langword="null"/>
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="name">类型名称</param>
    public ITasonTypeInfo? GetType<T>(string name) where T : notnull
        => GetType(name, typeof(T));

    /// <summary>
    /// 根据指定类型获取<see cref="ITasonTypeInfo"/>，若未注册返回<see langword="null"/>
    /// </summary>
    /// <param name="name">类型名称</param>
    /// <param name="type">类型</param>
    public ITasonTypeInfo? GetType(string name, Type type)
    {
        if (!types.TryGetValue(name, out var entry))
        {
            return null;
        };

        return entry.Types.FirstOrDefault(t => t.Type.IsAssignableFrom(type));
    }

    /// <summary>
    /// 根据指定对象获取<see cref="ITasonTypeInfo"/>，若未注册返回<see langword="null"/>
    /// </summary>
    /// <param name="name">类型名称</param>
    /// <param name="obj">类型实例对象</param>
    public ITasonTypeInfo? GetType(string name, object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        if (!types.TryGetValue(name, out var entry))
        {
            return null;
        };

        return entry.Types.FirstOrDefault(t => t.Type.IsAssignableFrom(obj.GetType()));
    }

    /// <summary>
    /// 获取指定名称类型默认实现的<see cref="ITasonTypeInfo"/>，若未注册返回<see langword="null"/>
    /// </summary>
    /// <param name="name">类型名称</param>
    public ITasonTypeInfo? GetDefaultType(string name)
    {
        if (!types.TryGetValue(name, out var entry))
        {
            return null;
        };
        return entry.Types.FirstOrDefault();
    }

    /// <summary>
    /// 获取指定名称类型所有实现的<see cref="ITasonTypeInfo"/>，若未注册返回空数组
    /// </summary>
    /// <param name="name">类型名称</param>
    public ITasonTypeInfo[] GetAllTypes(string name)
    {
        if (!types.TryGetValue(name, out var entry))
        {
            return [];
        };
        return [.. entry.Types];
    }


    #endregion


    public object CreateInstance(string typeName, Dictionary<string, object?> arg) {
        if (GetDefaultType(typeName) is not ITasonObjectType type)
        {
            throw new ArgumentException($"Unregistered type: {typeName}");
        }
        return CreateInstance(type, arg);
    }
    public object CreateInstance(string typeName, string arg) {
        if (GetDefaultType(typeName) is not ITasonScalarType type)
        {
            throw new ArgumentException($"Unregistered type: {typeName}");
        }
        return CreateInstance(type, arg);
    }

    public object CreateInstance(ITasonObjectType type, Dictionary<string, object?> arg) {
        return null!;
    }
    public object CreateInstance(ITasonScalarType type, string arg) {
        return null!;
    }


    private TasonRegistryEntry GetEntry(string name)
    {
        if (!types.TryGetValue(name, out var entry))
        {
            entry = new(name, []);
            types[name] = entry;
        }
        return entry;
    }
}
