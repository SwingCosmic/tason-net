using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TASON.Metadata;
using TASON.Serialization;
using static TASON.Util.ReflectionHelpers;

namespace TASON.Util;

internal enum EnumerableType
{
    Enumerable = 0,
    Dictionary = 1,
    AsyncEnumerable = 2,
    NonGenericEnumerable = 3,
    NonGenericDictionary = 4,
    Not = -1,
}

internal delegate IEnumerable<E> CreateCollection<E>(E[] array);

internal static class ReflectionHelpers
{
    public const string UseMakeGenericMethod = "该方法使用MethodInfo.MakeGenericMethod()";
    public const string UseMakeGenericType = "该方法使用Type.MakeGenericType()";

    public static EnumerableType IsEnumerable(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type, 
        out Type? elementType, out Type? keyType)
    {
        keyType = null;

        var interfaces = type.GetInterfaces();
        var i = interfaces.FirstOrDefault(t => t.Name.StartsWith("IEnumerable`1"));
        if (i is null)
        {
            elementType = null;

            if (!typeof(IEnumerable).IsAssignableFrom(type))
            {
                return EnumerableType.Not;
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return EnumerableType.NonGenericDictionary;
            }

            return EnumerableType.NonGenericEnumerable;
        }

        var dict = interfaces.FirstOrDefault(t => t.Name.StartsWith("IDictionary`2"));
        if (dict is not null)
        {
            elementType = dict.GenericTypeArguments[1];
            keyType = dict.GenericTypeArguments[0];
            return EnumerableType.Dictionary;
        }

        var asyncI = interfaces.FirstOrDefault(t => t.Name.StartsWith("IAsyncEnumerable`1"));
        if (asyncI is not null)
        {
            elementType = asyncI.GenericTypeArguments[0];
            return EnumerableType.AsyncEnumerable;
        }

        elementType = i.GenericTypeArguments[0];
        return EnumerableType.Enumerable;
    }

    public static bool IsValueTuple(Type type, out Type[] types)
    {
        types = [];
        if (!type.IsGenericType || !type.IsValueType) return false;
        
        if (!type.Name.StartsWith("ValueTuple`")) return false;
        
        types = type.GetGenericArguments();
        return true;
    }

    public static bool IsNullable(Type type, [NotNullWhen(true)]out Type? structType)
    {
        structType = null;
        if (!type.IsGenericType)  return false;

        var g = type.GetGenericTypeDefinition();
        if (g != typeof(Nullable<>)) return false;

        structType = type.GetGenericArguments()[0];
        return true;
    }

    /// <summary>
    /// 返回一个工厂方法，将<typeparamref name="E"/>类型的数组转换为集合类型<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">待转换的集合类型，可以是接口或是具体类，支持泛型和非泛型的版本</typeparam>
    /// <typeparam name="E">元素类型</typeparam>
    /// <returns>转换工厂方法，如果找不到则返回<see langword="null"/></returns>
    /// <exception cref="NotSupportedException">该类型没有符合要求的构造函数来创建</exception>


    public static CreateCollection<E>? GetEnumerableFactory<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T,
        E
    >() where T : IEnumerable<E>
    {
        var type = typeof(T);

        if (type.IsInterface)
        {
            // 处理接口类型
            if (type.IsAssignableFrom(typeof(List<E>)))
                return a => new List<E>(a);
            else if (type.IsAssignableFrom(typeof(HashSet<E>)))
                return a => new HashSet<E>(a);
            else
                return null;
        }
        else
        {
            // 处理具体类型，检查是否为已知的具体集合类型
            if (type == typeof(ReadOnlyCollection<E>))
                return a => new ReadOnlyCollection<E>(a);
            else if (type == typeof(HashSet<E>))
                return a => new HashSet<E>(a);
            else if (type == typeof(Collection<E>))
                return a => new Collection<E>(a);
            else if (type == typeof(List<E>))
                return a => new List<E>(a);
            else
            {
                // 查找构造函数：优先接受IEnumerable<E>，其次是E[]，最后是无参构造
                var ctorWithEnumerable = type.GetConstructor([typeof(IEnumerable<E>)]);
                if (ctorWithEnumerable != null)
                    return a => (IEnumerable<E>)ctorWithEnumerable.Invoke([a]);

                var ctorWithArray = type.GetConstructor([typeof(E[])]);
                if (ctorWithArray != null)
                    return a => (IEnumerable<E>)ctorWithArray.Invoke([a]);

                // 尝试无参构造函数并填充元素
                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor != null)
                {
                    return a =>
                    {
                        var ret = (IEnumerable<E>)ctor.Invoke(null);
                        if (ret is ISet<E> set)
                        {
                            foreach (var item in a) set.Add(item);
                        }
                        else if (ret is ICollection<E> coll)
                        {
                            foreach (var item in a) coll.Add(item);
                        }
                        else
                        {
                            throw new NotSupportedException($"Type {type.Name} does not support adding elements.");
                        }
                        return ret;
                    };
                }
                else
                {
                    return null; // 无合适构造函数
                }
            }
        }
    }



    public static IDictionary<K, V> CreateDictionary<K, V>(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type
    ) where K : notnull
    {
        IDictionary<K, V> obj;
        if (type.GetConstructor([]) is ConstructorInfo ctor)
            obj = (IDictionary<K, V>)ctor.Invoke([]);
        else if (type.IsAssignableFrom(typeof(Dictionary<K, V>)))
            obj = new Dictionary<K, V>();
        else
            throw new NotSupportedException($"Cannot deserialize TASON dictionary to type '{type.Name}'.Try creating your own type instance instead.");
        
        return obj;
    }

    [RequiresUnreferencedCode(UseMakeGenericMethod)]
    [RequiresDynamicCode(UseMakeGenericMethod)]
    public static R? CallGeneric<R>(this MethodInfo genericMethod, Type[] genericArgs, object? thisArg, object?[] args)
    {
        var m = genericMethod.MakeGenericMethod(genericArgs);
        return (R?)m.Invoke(thisArg, BindingFlags.DoNotWrapExceptions, null, args, null);
    }

    [RequiresUnreferencedCode(UseMakeGenericMethod)]
    [RequiresDynamicCode(UseMakeGenericMethod)]
    public static void CallGeneric(this MethodInfo genericMethod, Type[] genericArgs, object? thisArg, object?[] args)
    {
        var m = genericMethod.MakeGenericMethod(genericArgs);
        m.Invoke(thisArg, BindingFlags.DoNotWrapExceptions, null, args, null);
    }


    public static bool CanDirectConstruct(Type type)
    {
        return type.IsClass && 
            !type.IsAbstract && 
            !type.IsGenericType && 
            type.GetConstructor([]) is not null;
    }

}
