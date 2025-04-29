using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Antlr4.Runtime.Tree;
using TASON.Grammar;
using TASON.Metadata;
using TASON.Types;
using TASON.Util;
using KV = System.Collections.Generic.Dictionary<string, object?>;
using IKV = System.Collections.Generic.IDictionary<string, object?>;

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
    
    static readonly MethodInfo typedDictMethod = ReflectionHelpers
        .MethodOf((TasonVisitor v) => v.TypedDictionary<object>(default!, null!))
        .GetGenericMethodDefinition(); 
    static readonly MethodInfo typedDictArgMethod = ReflectionHelpers
        .MethodOf((TasonVisitor v) => v.TypedDictionaryArg<object, object>(default!, null!))
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

        return (T)MaybeTypeInstanceValue(typeInstanceValue, typeof(T));
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
        var isNullableStruct = false;
        if (type.IsValueType)
        {
            isNullableStruct = ReflectionHelpers.IsNullable(type, out var realType);
            if (isNullableStruct) type = realType!;
        }

        if (ctx is TASONParser.NullValueContext)
        {
            if (type.IsValueType && !isNullableStruct)
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
            if (ReflectionHelpers.CanDirectConstruct(type))
            {
                return typedObjectMethod.CallGeneric<object>([type], this, [objectValue.@object()]);
            }
            var enumerableType = ReflectionHelpers.IsEnumerable(type, out var elementType, out var keyType);
            if (enumerableType == EnumerableType.Dictionary)
            {
                if (keyType != typeof(string))
                {
                    throw new InvalidOperationException("Cannot deserialize plain object to a Dictionary with non-string keys");
                }
                return typedDictMethod.CallGeneric<object>([elementType!], this, [objectValue.@object(), type]);
            }
            throw new InvalidOperationException($"Cannot deserialize plain object to type '{type}', please register type instance instead.");
        } 
        else if (ctx is TASONParser.TypeInstanceValueContext typeInstanceValue)
        {
            return MaybeTypeInstanceValue(typeInstanceValue, type);
        }
        throw new ArgumentException($"Unsupported value type: {ctx.GetType().Name}");
    }



    ValueType TypedNumberValue(TASONParser.NumberValueContext ctx, Type type)
    {
        var text = ctx.number().GetText();
        if (!NumberMetadata.TryGetClrType(type, out var _))
        {
            if (!type.IsEnum)
            {
                throw new InvalidCastException($"Cannot cast type '{type.Name}' to number");
            }
            else
            {
                var numType = Enum.GetUnderlyingType(type);
                var value = NumberMetadata.Deserialize(numType, text, options);
                return (ValueType)Enum.ToObject(type, value);
            }
        }

        return NumberMetadata.Deserialize(type, text, options);
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
        
    internal object MaybeTypeInstanceValue(TASONParser.TypeInstanceValueContext ctx, Type type)
    {
        var typeInstance = ctx.typeInstance();
        return typeInstance switch
        {
            TASONParser.ScalarTypeInstanceContext scalarType => TypedScalarTypeInstance(scalarType, type),
            TASONParser.ObjectTypeInstanceContext objectType => TypedObjectTypeInstance(objectType, type),
            _ => throw new ArgumentException($"Unsupported type instance type: {typeInstance.GetType().Name}"),
        };
    }

    internal object TypedScalarTypeInstance(TASONParser.ScalarTypeInstanceContext ctx, Type type)
    {
        var typeName = ctx.IDENTIFIER().GetText();
        var str = GetTextValue(ctx.STRING());

        return CreateTypeInstance(typeName, str, type);
    }

    internal object TypedObjectTypeInstance(TASONParser.ObjectTypeInstanceContext ctx, Type type)
    {
        var typeName = ctx.IDENTIFIER().GetText();
        if (options.UseBuiltinDictionary && typeName == DictionaryType.TypeName)
        {
            var enumerableType = ReflectionHelpers.IsEnumerable(type, out var elementType, out var keyType);
            if (enumerableType != EnumerableType.Dictionary)
            {
                throw new InvalidCastException($"Type '{type.Name}' is not a Dictionary");
            }
            return typedDictArgMethod.CallGeneric<object>([keyType!, elementType!], this, [ctx.@object(), type])!;
        }
        return CreateTypeInstance(typeName, ctx.@object(), type);
    }

    internal T TypedObject<T>(TASONParser.ObjectContext ctx) where T : class, new()
    {
        var ret = new T();
        IKV? extra = null;

        var meta = TasonTypeMetadataProvider.GetMetadata<T>();
        if (meta.ExtraFieldsProperty is KeyValuePair<string, PropertyInfo> extraProp)
        {
            extra = ReflectionHelpers.CreateDictionary<string, object?>(extraProp.Value.PropertyType);
            extraProp.Value.SetValue(ret, extra);
        }

        FillObjectProperties(ctx, meta, (_, prop, value) => prop.SetValue(ret, value), extra);

        return ret;
    }
    
    internal KV TypedObjectArg(TASONParser.ObjectContext ctx, Type type)
    {
        var meta = TasonTypeMetadataProvider.GetMetadata(type);
        var obj = new KV();

        FillObjectProperties(ctx, meta, (key, _, value) => obj[key] = value, obj);

        return obj;
    }   

    internal void FillObjectProperties(
        TASONParser.ObjectContext ctx,
        ITasonTypeMetadata meta,
        Action<string, PropertyInfo, object?> setValue,
        IKV? extra)
    {
        var props = meta.Properties;

        var propSet = new HashSet<string>();
        foreach (var pair in ctx.pair())
        {
            var key = Key(pair.key());
            if (!props.TryGetValue(key, out var prop))
            {
                if (extra is not null)
                {
                    var autoValue = ValueContext(pair.value());
                    extra[key] = autoValue;
                }
                continue;
            }

            if (!propSet.Add(key) && !options.AllowDuplicatedKeys)
                throw new ArgumentException($"Duplicate key '{key}' in object");

            if (!prop.CanWrite)
                continue;

            var value = TypedValueContext(pair.value(), prop.PropertyType);
            setValue(key, prop, value);
        }
    }


    internal IDictionary<K, V> TypedDictionaryArg<K, V>(TASONParser.ObjectContext ctx, Type type) where K : notnull
    {
        var obj = ReflectionHelpers.CreateDictionary<K, V>(type);

        TASONParser.ArrayContext? arrayContext = null;
        foreach (var pair in ctx.pair())
        {
            var key = Key(pair.key());
            if (key != nameof(ObjectDictionary<K, V>.keyValuePairs))
                continue;

            var valueContext = pair.value();
            if (valueContext is TASONParser.ArrayValueContext arrayValue)
            {
                arrayContext = arrayValue.array();
                break;
            }     
        }

        if (arrayContext is null) goto INVALID;

        var kt = typeof(K);
        var vt = typeof(V);

        foreach (var item in arrayContext.value())
        {
            if (item is TASONParser.ArrayValueContext arrayValue)
            {
                var pair = arrayValue.array().value();
                if (pair.Length != 2)
                    goto INVALID;

                K key = (K)TypedValueContext(pair[0], kt)!;
                V value = (V)TypedValueContext(pair[1], vt)!;
                obj[key] =value;
            } else
            {
                goto INVALID;
            }
        }

        return obj;

INVALID:
        throw new InvalidOperationException("Invalid object arg for TASON Dictionary");
    }

    internal IDictionary<string, V> TypedDictionary<V>(TASONParser.ObjectContext ctx, Type type)
    {
        var obj = ReflectionHelpers.CreateDictionary<string, V>(type);

        var propSet = new HashSet<string>();
        foreach (var pair in ctx.pair())
        {
            var key = Key(pair.key());

            if (!propSet.Add(key) && !options.AllowDuplicatedKeys)
                throw new ArgumentException($"Duplicate key '{key}' in object");

            var value = (V)TypedValueContext(pair.value(), typeof(V))!;
            obj[key] = value;
        }
        return obj;
    }

    private object CreateTypeInstance(string typeName, TASONParser.ObjectContext obj, Type implType)
    {
        if (registry.GetType(typeName, implType) is not ITasonObjectType typeInfo)
        {
            throw new ArgumentException($"Unregistered type: {typeName}(Implementation: {implType.FullName})");
        }

        return registry.CreateInstance(typeInfo, TypedObjectArg(obj, typeInfo.Type));
    }

    private object CreateTypeInstance(string typeName, string value, Type implType)
    {
        if (registry.GetType(typeName, implType) is not ITasonScalarType typeInfo)
        {
            throw new ArgumentException($"Unregistered type: {typeName}(Implementation: {implType.FullName})");
        }

        return registry.CreateInstance(typeInfo, value);
    }


    static void CheckType(Type type, Type expect, string? displayName = null)
    {
        if (type != expect)
        {
            throw new InvalidCastException($"Cannot cast type '{type.Name}' to '{displayName ?? expect.Name}'");
        }
    }

}
