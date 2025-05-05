using System.Reflection;
using TASON.Grammar;
using TASON.Serialization;
using TASON.Util;

namespace TASON.Types;

/// <summary>
/// 表示一个键不是字符串的字典
/// </summary>
public sealed record class ObjectDictionary<K, V> where K : notnull
{

#pragma warning disable IDE1006 // 命名样式
    /// <summary>
    /// 储存键值对的列表
    /// </summary>
    public List<(K Key, V Value)> pairs { get; set; } = new();
#pragma warning restore IDE1006
    public ObjectDictionary() { }

    public ObjectDictionary(IEnumerable<(K Key, V Value)> pairs)
    {
        this.pairs = pairs.ToList();
    }

    /// <summary>
    /// 从指定的字典创建ObjectDictionary
    /// </summary>
    /// <param name="dict">字典</param>
    public static ObjectDictionary<K, V> From(IDictionary<K, V> dict)
    {
        return new ObjectDictionary<K, V>
        {
            pairs = dict
                .Select(p => (p.Key!, p.Value!))
                .ToList(),
        };
    }


    /// <summary>
    /// 将<see cref="ObjectDictionary{K, V}"/>转换成<see cref="Dictionary{K, V}"/>
    /// </summary>
    public Dictionary<K, V> ToDictionary()
    {
        return pairs.ToDictionary(p => p.Key!, p => p.Value!);
    }
}

internal static class DictionaryType
{
    /// <summary>
    /// 对应TASON内置类型的名称
    /// </summary>
    public const string TypeName = "Dictionary";

    public static Dictionary<string, object?> SerializeToArg<K, V>(this ObjectDictionary<K, V> objDict) where K : notnull
    {
        return new()
        {
            [nameof(ObjectDictionary<K, V>.pairs)] = objDict.pairs
                .Select(p => new object[] { p.Key!, p.Value! })
                .ToArray(),
        };
    }

}