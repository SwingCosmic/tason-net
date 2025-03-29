
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TASON.Util;
using static TASON.Util.ReflectionHelpers;
namespace TASON;

public partial class TasonGenerator
{
    static readonly MethodInfo arrayMethod = MethodOf((TasonGenerator t) => t.ArrayValue<int>(null!))
        .GetGenericMethodDefinition();
    static readonly MethodInfo dictionaryMethod = MethodOf((TasonGenerator t) => t.DictionaryValue<string, int>(null!))
        .GetGenericMethodDefinition();


    bool TryGetArrayValue(object value, [NotNullWhen(true)] out string? result)
    {
        var type = value.GetType();
        var isEnumerable = IsEnumerable(type, out var elementType, out var keyType);
        switch (isEnumerable)
        {
            case EnumerableType.AsyncEnumerable:
                throw new NotSupportedException("Cannot serialize async enumerable type");
            case EnumerableType.NonGenericDictionary:
                result = DictionaryValue((value as IDictionary)!);
                break;
            case EnumerableType.NonGenericEnumerable:
                result = ArrayValue((value as IEnumerable)!);
                break;
            case EnumerableType.Dictionary:
                result = CallGenericMethod<string>(dictionaryMethod, [keyType!, elementType!], this, [value]);
                break;
            case EnumerableType.Enumerable:
                result = CallGenericMethod<string>(arrayMethod, [elementType!], this, [value]);
                break;
            default:
                result = null;
                return false;
        }
        return true;
    }


    string ArrayValue(IEnumerable value)
    {
        string[] result;
        indentLevel++;
        {
            CheckDepth();
            result = value
                .Cast<object>()
                .Select(e => 
                {
                    var v = Value(e) ?? throw new InvalidOperationException("Non-serializable value");
                    return Indent() + Value(e);
                })
                .ToArray();
        }
        indentLevel--;

        if (result.Length == 0) return "[]";

        if (options.Indent is null)
            return $"[{string.Join(',', result)}]";
        else 
            return $"[\n{string.Join(",\n", result)}\n{Indent()}]";
        
    }
    string ArrayValue<T>(IEnumerable<T> value)
    {
        string[] result;
        indentLevel++;
        {
            CheckDepth();
            result = value
                .Select(e => Indent() + Value(e)!)
                .ToArray();
        }
        indentLevel--;

        if (result.Length == 0) return "[]";

        if (options.Indent is null)
            return $"[{string.Join(',', result)}]";
        else 
            return $"[\n{string.Join(",\n", result)}\n{Indent()}]";
        
    }


}