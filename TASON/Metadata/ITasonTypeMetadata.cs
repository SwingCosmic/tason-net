using System.Reflection;
using System.Runtime.CompilerServices;
using TASON.Serialization;

namespace TASON.Metadata;

/// <summary>
/// 提供指定类型的反射元数据
/// </summary>
public interface ITasonTypeMetadata
{
    /// <summary>
    /// 获取关联的类型
    /// </summary>
    Type Type { get; }

    /// <summary>类的可序列化属性</summary>
    Dictionary<string, PropertyInfo> Properties { get; }
    
    /// <summary>类的可序列化字段</summary>
    Dictionary<string, FieldInfo> Fields { get; }

    /// <summary>类的额外字段属性，通常通过<see cref="TasonExtraMemberAttribute"/>标记</summary>
    KeyValuePair<string, PropertyInfo>? ExtraMemberProperty { get; }
}