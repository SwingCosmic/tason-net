
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using static TASON.ReflectionHelpers;
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

    string TypeInstanceValue(object value, ITasonTypeInfo type)
    {
        return "";
    }

    string Key(string key)
    {
        if (identifierPattern.IsMatch(key))
            return key;
        return StringValue(key);
    }

    // 非泛型字典允许不可序列化的key和value，需要丢弃
    bool TryGetPair(DictionaryEntry keyValue, out (string Key, string Value) pair)
    {
        if (keyValue.Key is not string key)
        {
            pair = default;
            return false;
        }

        var value = Value(keyValue.Value, ValueScope.ObjectValue);
        if (value == null)
        {
            pair = default;
            return false;
        }
        pair = (Key(key), value);
        return true;
    }

    // 泛型字典的key, value类型均已校验，不会遇到不可序列化的类型
    (string Key, string Value) Pair<K, V>(KeyValuePair<K, V> keyValue)
    {
        if (keyValue.Key is not string key)
        {
            // 仅用于过类型校验，不应该走到这里
            throw new InvalidOperationException("Key must be a string");
        }
        return (Key(key), Value(keyValue.Value, ValueScope.ObjectValue)!);
    }

    string MaybeObjectValue(object value, Type type)
    {
        ThrowIfBanTypes(type);
        return "";
    }

    string ObjectValue(object value, Type type) 
    {
        return "{}";
    }

    string DictionaryValue(IDictionary dict)
    {
        List<string> pairs = new(); 
        indentLevel++;
        {
            CheckDepth();
            foreach (DictionaryEntry pair in dict)
            {
                if (!TryGetPair(pair, out var p)) continue;
                pairs.Add($"{Indent()}{p.Key}:{Space()}{p.Value}");
            }
        }
        indentLevel--;

        if (pairs.Count == 0) return "{}";

        if (options.Indent is null)
            return $"{{{string.Join(',', pairs)}}}";
        else
            return $"{{\n{string.Join(",\n", pairs)}\n{Indent()}}}";
    }

    string DictionaryValue<K, V>(IDictionary<K, V> dict)
    {
        if (typeof(K) != typeof(string))
            throw new NotSupportedException("Cannot serialize Dictionary with non string key");
        ThrowIfBanTypes(typeof(V));

        List<string> pairs = new(); 
        indentLevel++;
        {
            CheckDepth();
            foreach (var pair in dict)
            {
                var p = Pair(pair);
                pairs.Add($"{Indent()}{p.Key}:{Space()}{p.Value}");
            }
        }
        indentLevel--;

        if (pairs.Count == 0) return "{}";

        if (options.Indent is null)
            return $"{{{string.Join(',', pairs)}}}";
        else
            return $"{{\n{string.Join(",\n", pairs)}\n{Indent()}}}";
    }
}