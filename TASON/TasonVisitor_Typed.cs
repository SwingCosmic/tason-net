using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using TASON.Grammar;
using TASON.Metadata;
using TASON.Util;

namespace TASON;

public partial class TasonVisitor
{

    static readonly MethodInfo typedArrayValueMethod = ReflectionHelpers
        .MethodOf((TasonVisitor v) => v.TypedArray<int>(default!))
        .GetGenericMethodDefinition();
    
    static readonly MethodInfo typedCollectionMethod = ReflectionHelpers
        .MethodOf((TasonVisitor v) => v.TypedCollection<int>(default!, default!))
        .GetGenericMethodDefinition();     
    
    static readonly MethodInfo typedObjectMethod = ReflectionHelpers
        .MethodOf((TasonVisitor v) => v.TypedObject<object>(default!))
        .GetGenericMethodDefinition(); 
    
    static readonly MethodInfo enumerableFactoryMethod = ReflectionHelpers
        .MethodOf(() => ReflectionHelpers.GetEnumerableFactory<int[], int>())
        .GetGenericMethodDefinition();

    /// <summary>
    /// 遍历parse tree并将其代表的TypeInstanceValue以<typeparamref name="T"/>类型反序列化
    /// </summary>
    /// <typeparam name="T">TASON TypeInstance对应的CLR类型</typeparam>
    /// <param name="context">The parse tree.</param>
    /// <returns>反序列化的<typeparamref name="T"/>类型实例</returns>
    /// <exception cref="InvalidOperationException">Parse tree不代表TypeInstanceValue</exception>
    public T StartTypeInstanceValue<T>(TASONParser.StartContext context) where T : notnull
    {
        var ctx = context.value();
        if (ctx is not TASONParser.TypeInstanceValueContext typeInstanceValue)
        {
            throw new InvalidOperationException($"{ctx.GetType().Name} is not a TypeInstanceValue");
        }

        return (T)TypeInstanceValue(typeInstanceValue);
    }

    /// <summary>
    /// 遍历parse tree并将其代表的对象尝试匹配<typeparamref name="T"/>类型进行反序列化
    /// </summary>
    /// <typeparam name="T">要反序列化的类型</typeparam>
    /// <param name="context">The parse tree.</param>
    /// <returns>反序列化的<typeparamref name="T"/>类型实例</returns>
    /// <exception cref="InvalidOperationException">类型匹配失败</exception>
    public T? StartDeserialize<T>(TASONParser.StartContext context) where T : notnull
    {
        return (T?)StartDeserialize(context, typeof(T));
    }

    /// <summary>
    /// 遍历parse tree并将其代表的对象尝试匹配指定类型进行反序列化
    /// </summary>
    /// <param name="context">The parse tree.</param>
    /// <param name="type">要反序列化的类型</param>
    /// <returns>反序列化的类型实例</returns>
    /// <exception cref="InvalidOperationException">类型匹配失败</exception>    
    public object? StartDeserialize(TASONParser.StartContext context, Type type)
    {
        var ctx = context.value();
        if (type == typeof(object))
            return ValueContext(ctx);
        else
            return TypedValueContext(ctx, type);
    }


    internal object? TypedValueContext(TASONParser.ValueContext ctx, Type type)
    {
        if (ctx is TASONParser.NullValueContext)
        {
            if (type.IsValueType)
            {
                throw new InvalidCastException($"Cannot cast null to struct {type.Name}");
            }
            return null;
        }
        else if (ctx is TASONParser.BooleanValueContext booleanValue)
        {
            CheckType(type, typeof(bool), "boolean");
            return BooleanValue(booleanValue);
        }
        else if (ctx is TASONParser.StringValueContext stringValue)
        {
            CheckType(type, typeof(string), "string");
            return StringValue(stringValue);
        }
        else if (ctx is TASONParser.NumberValueContext numberValue)
        {
            return TypedNumberValue(numberValue, type);
        }
        else if (ctx is TASONParser.ArrayValueContext arrayValue)
        {
            var array = arrayValue.array().value();
            var enumerableType = ReflectionHelpers.IsEnumerable(type, out var elementType, out _);
            if (enumerableType == EnumerableType.Enumerable)
            {
                if (type.IsArray)
                {
                    return typedArrayValueMethod.CallGeneric<Array>([elementType!], this, [array]);
                }
                return typedCollectionMethod.CallGeneric<IEnumerable>([elementType!], this, [array, type]);
            }
            else if (enumerableType == EnumerableType.NonGenericEnumerable)
            {
                return NonGenericCollection(array, type);
            }
            else
            {
                throw new InvalidCastException($"Type '{type.Name}' is not an Enumerable");
            }
        } 
        else if (ctx is TASONParser.ObjectValueContext objectValue)
        {
            if (!ReflectionHelpers.CanDirectConstruct(type))
            {
                throw new InvalidOperationException($"Cannot deserialize plain object to type '{type}', please register type instance instead.");
            }
            return typedObjectMethod.CallGeneric<object>([type], this, [objectValue.@object()]);
        } 
        else if (ctx is TASONParser.TypeInstanceValueContext typeInstanceValue)
        {
            return TypeInstanceValue(typeInstanceValue);
        }
        throw new ArgumentException($"Unsupported value type: {ctx.GetType().Name}");
    }



    ValueType TypedNumberValue(TASONParser.NumberValueContext ctx, Type type)
    {
        if (!NumberMetadata.TryGetClrType(type, out var _))
        {
            throw new InvalidCastException($"Cannot cast type '{type.Name}' to number");
        }

        return NumberMetadata.Deserialize(type, ctx.number().GetText(), options);
    }

    T[] TypedArray<T>(TASONParser.ValueContext[] array)
    {
        var type = typeof(T);
        var ret = new T[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            ret[i] = (T)TypedValueContext(array[i], type)!;
        }
        return ret;
    }

    IEnumerable<T> TypedCollection<T>(TASONParser.ValueContext[] array, Type collectionType)
    {
        var data = TypedArray<T>(array);
        var factory = enumerableFactoryMethod.CallGeneric<CreateCollection<T>>([collectionType, typeof(T)], null, []) 
            ?? throw new NotSupportedException($"Cannot deserialize TASON array to type '{collectionType.Name}'.Try creating your own type instance instead.");
        return factory(data);
    }

    IEnumerable NonGenericCollection(TASONParser.ValueContext[] array, Type collectionType)
    {
        var data = Array(array);
        if (collectionType == typeof(ArrayList))
            return new ArrayList(data);
        else if (collectionType == typeof(Queue))
            return new Queue(data);
        else if (collectionType == typeof(Stack))
            return new Stack(data);
        else
            throw new NotSupportedException($"Non-generic collection type '{collectionType}' is not supported.Shoud not use non-generic collection type any more.");
    }

    internal T TypedObject<T>(TASONParser.ObjectContext ctx) where T : class, new()
    {
        var obj = new T();
        var propSet = new HashSet<string>();
        foreach (var pair in ctx.pair())
        {
            var key = Key(pair.key());
            if (!ClassPropertyMetadata.Cache<T>.Properties.TryGetValue(key, out var prop))
                continue;
            
            if (!propSet.Add(key) && !options.AllowDuplicatedKeys)
                throw new ArgumentException($"Duplicate key '{key}' in object");

            if (!prop.CanWrite)
                continue;

            var value = TypedValueContext(pair.value(), prop.PropertyType);
            prop.SetValue(obj, value);
        }
        return obj;
    }
    
    internal Dictionary<string, object?> TypedObjectArg(TASONParser.ObjectContext ctx, Type type)
    {
        var obj = new Dictionary<string, object?>();
        var properties = ReflectionHelpers.GetClassProperties(type);
        var propSet = new HashSet<string>();
        foreach (var pair in ctx.pair())
        {
            var key = Key(pair.key());
            if (!properties.TryGetValue(key, out var prop))
                continue;
            
            if (!propSet.Add(key) && !options.AllowDuplicatedKeys)
                throw new ArgumentException($"Duplicate key '{key}' in object");

            if (!prop.CanWrite)
                continue;

            var value = TypedValueContext(pair.value(), prop.PropertyType);
            obj[key] = value;
        }
        return obj;
    }


    static void CheckType(Type type, Type expect, string? displayName = null)
    {
        if (type != expect)
        {
            throw new InvalidCastException($"Cannot cast type '{type.Name}' to '{displayName ?? expect.Name}'");
        }
    }
}
