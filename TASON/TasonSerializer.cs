using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASON;

/// <summary>
/// TASON序列化器
/// </summary>
public class TasonSerializer
{
    /// <summary>
    /// 序列化选项
    /// </summary>
    public SerializerOptions Options { get; }
    /// <summary>
    /// 类型注册表
    /// </summary>
    public TasonTypeRegistry Registry { get; }

    /// <summary>
    /// 默认TASON序列化器
    /// </summary>
    public static TasonSerializer Default { get; } = new TasonSerializer();

    /// <summary>
    /// 创建<see cref="TasonSerializer"/>的新实例
    /// </summary>
    /// <param name="options">序列化选项</param>
    /// <param name="registry">类型注册表</param>
    public TasonSerializer(SerializerOptions? options = null, TasonTypeRegistry? registry = null)
    {
        options ??= new SerializerOptions();
        Options = options;

        registry ??= new TasonTypeRegistry();
        Registry = registry;
    }

    public object? Deserialize(string text)
    {
        throw new NotImplementedException();
    }

    public string Serialize(object? value)
    {
        throw new NotImplementedException();
    }
}
