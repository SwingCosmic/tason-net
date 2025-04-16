
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


    bool TryWriteArrayValue(object value)
    {
        var type = value.GetType();
        var isEnumerable = IsEnumerable(type, out var elementType, out var keyType);
        switch (isEnumerable)
        {
            case EnumerableType.AsyncEnumerable:
                throw new NotSupportedException("Cannot serialize async enumerable type");
            case EnumerableType.NonGenericDictionary:
                DictionaryValue((value as IDictionary)!);
                break;
            case EnumerableType.NonGenericEnumerable:
                ArrayValue((value as IEnumerable)!);
                break;
            case EnumerableType.Dictionary:
                dictionaryMethod.CallGeneric([keyType!, elementType!], this, [value]);
                break;
            case EnumerableType.Enumerable:
                arrayMethod.CallGeneric([elementType!], this, [value]);
                break;
            default:
                return false;
        }
        return true;
    }


    void ArrayValue(IEnumerable value)
    {
        writer.WriteStartArray();
        {
            writer.WriteJoin(v => 
            {
                writer.WriteArrayItem(() => 
                {
                    if (!Value(v))
                    {
                        throw new InvalidOperationException("Non-serializable value");
                    }
                });
                return true;
            }, value.Cast<object>().ToArray());
        }
        writer.WriteEndArray();
    }

    void ArrayValue<T>(IEnumerable<T> value)
    {
        writer.WriteStartArray();
        {
            writer.WriteJoin(v => 
            {
                writer.WriteArrayItem(() => Value(v));
                return true;
            }, value.ToArray());
        }
        writer.WriteEndArray();
    }


}