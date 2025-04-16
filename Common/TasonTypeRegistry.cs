using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using TASON.Serialization;
using TASON.Types;
using TASON.Util;

namespace TASON;

internal record class TasonRegistryEntry(string Name, List<ITasonTypeInfo> Types)
{

}

/// <summary>
/// TASON类型注册表。大多数情况下，不应该直接创建全新的注册表，
/// 而是从实例的<see cref="TasonSerializer.Registry"/>属性进行克隆
/// </summary>
/// <param name="options">选项</param>
public class TasonTypeRegistry
{
    Dictionary<string, TasonRegistryEntry> types = new();
    private readonly SerializerOptions options;

    public SerializerOptions Options => options;

    public TasonTypeRegistry(SerializerOptions options)
    {
        this.options = options;
        // 注册存在依赖关系，顺序必须依次为默认类型，别名，然后是鸭子类型
        foreach (var (name, type) in BuiltinTypes.Types)
        {
            RegisterType(name, type);
        }
        foreach (var (name, type) in BuiltinTypes.Aliases)
        {
            RegisterTypeAlias(name, type);
        }
        foreach (var (name, types) in BuiltinTypes.DuckTypes)
        {
            foreach (var type in types)
            {
                RegisterType(name, type);
            }
        }
    }

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
    /// <exception cref="TasonTypeNotFoundException">原有的类型未注册</exception>
    public void RegisterTypeAlias(string name, string originName) 
    {
        if (!types.TryGetValue(originName, out var entry))
        {
            throw new TasonTypeNotFoundException(originName);
        };
        types[name] = entry;
    }

    /// <summary>
    /// 通过反射创建并注册一个自动实现的<see cref="ITasonObjectType"/>
    /// </summary>
    /// <param name="type">要注册的类型，必须有无参构造函数</param>
    /// <returns>创建的<see cref="ITasonObjectType"/></returns>
    /// <exception cref="ArgumentException">类型不是类、是抽象类、是泛型类或者没有公共无参构造函数</exception>

    [RequiresUnreferencedCode("该方法使用Type.MakeGenericType()")]
    public ITasonObjectType CreateObjectType(Type type)
    {
        if (!ReflectionHelpers.CanDirectConstruct(type))
        {
            throw new ArgumentException("Invalid type");
        }

        var objType = typeof(TasonObjectType<>).MakeGenericType(type);
        var typeInfo = (ITasonObjectType)Activator.CreateInstance(objType)!;
        RegisterType(type.Name, typeInfo);
        return typeInfo;
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
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

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

    /// <summary>
    /// 尝试获取指定对象注册的TASON类型实例，包含名称
    /// </summary>
    /// <param name="value">待判断的对象</param>
    /// <param name="typeInfo">返回的包含名称的TASON类型实例</param>
    /// <returns>是否找到</returns>
    public bool TryGetTypeInfo(object value, [NotNullWhen(true)]out TasonNamedTypeInfo? typeInfo) 
    {
        if (value is ITasonTypeDiscriminator discriminator)
        {
            var name = discriminator.GetTypeName();
            if (GetType(name, value) is ITasonTypeInfo info)
            {
                typeInfo = new (name, info);
                return true;
            }
            throw new InvalidOperationException(
                $"Object returned its type '{name}' in type discriminator, which is not registered in the registry.");
        }

        var valueType = value.GetType();
        foreach (var entry in types)
        {
            foreach (var type in entry.Value.Types)
            {
                if (type.Type.IsAssignableFrom(valueType))
                {
                    typeInfo = new(entry.Key, type);
                    return true;
                }
            }
        }
        typeInfo = null;
        return false;
    }


    #endregion

    #region 创建类型实例

    /// <summary>
    /// 根据类型名称和表示对象属性的字典创建TASON对象类型实例
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <param name="arg">表示对象属性的字典</param>
    /// <returns>TASON对象类型实例</returns>
    /// <exception cref="TasonTypeNotFoundException">类型未注册</exception>
    /// <exception cref="ArgumentException">不是<see cref="ITasonObjectType" />类型</exception>
    public object CreateInstance(string typeName, Dictionary<string, object?> arg)
    {
        return GetDefaultType(typeName) switch
        {
            null => throw new TasonTypeNotFoundException(typeName),
            ITasonObjectType type => CreateInstance(type, arg),
            _ => throw new ArgumentException($"{typeName} is not an {nameof(ITasonObjectType)}"),
        };
    }
    /// <summary>
    /// 根据类型名称和表示标量的字符串创建TASON标量类型实例
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <param name="arg">表示标量的字符串</param>
    /// <returns>TASON标量类型实例</returns>
    /// <exception cref="TasonTypeNotFoundException">类型未注册</exception>
    /// <exception cref="ArgumentException">不是<see cref="ITasonScalarType" />类型</exception>
    public object CreateInstance(string typeName, string arg)
    {
        return GetDefaultType(typeName) switch
        {
            null => throw new TasonTypeNotFoundException(typeName),
            ITasonScalarType type => CreateInstance(type, arg),
            _ => throw new ArgumentException($"{typeName} is not an {nameof(ITasonScalarType)}"),
        };
    }
    /// <summary>
    /// 根据类型信息和表示对象属性的字典创建TASON对象类型实例
    /// </summary>
    /// <param name="type">TASON对象类型信息</param>
    /// <param name="arg">表示对象属性的字典</param>
    /// <returns>TASON对象类型实例</returns>
    public object CreateInstance(ITasonObjectType type, Dictionary<string, object?> arg)
    {
        return type.Deserialize(arg, options);
    }
    /// <summary>
    /// 根据类型信息和表示标量的字符串创建TASON标量类型实例
    /// </summary>
    /// <param name="type">TASON标量类型信息</param>
    /// <param name="arg">表示标量的字符串</param>
    /// <returns>TASON标量类型实例</returns>
    public object CreateInstance(ITasonScalarType type, string arg)
    {
        return type.Deserialize(arg, options);
    }

    #endregion

    public object SerializeToArg(string typeName, object value)
    {
        var type = GetType(typeName, value) ?? throw new TasonTypeNotFoundException(typeName);
        return type switch
        {
            ITasonScalarType scalar => SerializeToArg(scalar, value),
            ITasonObjectType obj => SerializeToArg(obj, value),
            _ => throw new InvalidOperationException(),
        };
    }       
    
    public string SerializeToArg(ITasonScalarType type, object value)
    {
        return type.Serialize(value, options);
    }    
    
    public Dictionary<string, object?> SerializeToArg(ITasonObjectType type, object value)
    {
        return type.Serialize(value, options);
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
