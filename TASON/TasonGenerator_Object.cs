
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TASON.Serialization;
using TASON.Types;
using TASON.Util;
using static TASON.Util.ReflectionHelpers;
namespace TASON;

public partial class TasonGenerator
{

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex PatternRegex();
    private static readonly Regex identifierPattern = PatternRegex();
#else
    private static readonly Regex identifierPattern = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
#endif

    void TypeInstanceValue(object value, ITasonScalarType type, string name)
    {
        var arg = registry.SerializeToArg(type, value);
        writer.WriteTypeInstance(name, () => writer.WriteString(arg));
    }
    
    void TypeInstanceValue(object value, ITasonObjectType type, string name)
    {
        var arg = registry.SerializeToArg(type, value);
        writer.WriteTypeInstance(name, () => DictionaryValueNoCheck(arg));
    }

    void TypeInstanceValue(object value, TasonNamedTypeInfo type)
    {
        if (type.TypeInfo is ITasonScalarType scalar)
        {
            TypeInstanceValue(value, scalar, type.Name);
            return;
        }
        else if (type.TypeInfo is ITasonObjectType obj)
        {
            TypeInstanceValue(value, obj, type.Name);
            return;
        } 
        throw new InvalidOperationException();
    }

    void Key(string key)
    {
        if (identifierPattern.IsMatch(key))
            writer.Write(key);
        else
            writer.WriteString(key);
    }

    // 非泛型字典允许不可序列化的key和value，需要丢弃
    bool TryWritePair(DictionaryEntry keyValue)
    {
        if (keyValue.Key is not string key)
        {
            return false;
        }

        if (keyValue.Value is not null && IsBanTypes(keyValue.Value.GetType()))
        {
            return false;
        }

        writer.WriteObjectPair(() => Key(key), () => Value(keyValue.Value, ValueScope.ObjectValue));
        return true;
    }

    // 泛型字典的key, value类型均已校验，不会遇到不可序列化的类型
    void Pair<K, V>(KeyValuePair<K, V> keyValue)
    {
        if (keyValue.Key is not string key)
        {
            // 仅用于过类型校验，不应该走到这里
            throw new InvalidOperationException("Key must be a string");
        }
        writer.WriteObjectPair(() => Key(key), () => Value(keyValue.Value, ValueScope.ObjectValue));
    }

    void MaybeObjectValue(object value, Type type)
    {
        ThrowIfBanTypes(type);
        if (registry.TryGetTypeInfo(value, out var typeInfo))
            TypeInstanceValue(value, (TasonNamedTypeInfo)typeInfo);
        else
            ObjectValue(value, type);
    }

    void ObjectValue(object value, Type type) 
    {
        var props = GetClassProperties(type);
        writer.WriteStartObject();
        {
            writer.CheckDepth();
            writer.Join(v => 
            {
                Pair(v);
                return true;
            }, 
            props.Select(p => 
            {
                var (key, propInfo) = p;
                return new KeyValuePair<string, object?>(key, propInfo.GetValue(value));
            }).ToArray());
        }
        writer.WriteEndObject();
    }

    void DictionaryValue(IDictionary dict)
    {
        writer.WriteStartObject();
        {
            writer.CheckDepth();
            writer.Join(v => 
            {
                if (!TryWritePair(v)) 
                    return false;
                return true;
            }, dict.Cast<DictionaryEntry>().ToArray());
        }
        writer.WriteEndObject();
    }

    void DictionaryValue<K, V>(IDictionary<K, V> dict)
    {
        if (options.UseBuiltinDictionary)
        {
            var objDict = ObjectDictionary<K, V>.From(dict);
            writer.WriteTypeInstance(DictionaryType.TypeName, 
                () => DictionaryValueNoCheck(objDict.SerializeToArg()));
        } else
        {
            DictionaryValueNoCheck(dict);
        }
    }

    /// <summary>
    /// 不检查<see cref="options"/>.UseBuiltinDictionary的版本。
    /// 防止在检查时，以及序列化<see cref="ITasonObjectType"/>的参数时循环调用
    /// </summary>
    void DictionaryValueNoCheck<K, V>(IDictionary<K, V> dict)
    {
        if (typeof(K) != typeof(string))
            throw new NotSupportedException("Cannot serialize Dictionary with non string key");
        ThrowIfBanTypes(typeof(V));

        writer.WriteStartObject();
        {
            writer.CheckDepth();
            writer.Join(v => 
            {
                Pair(v);
                return true;
            }, 
            dict.ToArray());
        }
        writer.WriteEndObject();
    }
}