using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

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

internal static class ReflectionHelpers
{

    public static EnumerableType IsEnumerable(Type type, out Type? elementType, out Type? keyType)
    {
        keyType = null;

        var interfaces = type.GetInterfaces();
        var i = interfaces.FirstOrDefault(t => t.Name.StartsWith("IEnumerable`1"));
        if (i is null)
        {
            elementType = null;

            var ni = interfaces.FirstOrDefault(t => t.Name == "IEnumerable");
            if (ni is null)
            {
                return EnumerableType.Not;
            }

            var ndict = interfaces.FirstOrDefault(t => t.Name == "IDictionary");
            if (ndict is not null)
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

    public static FieldInfo FieldOf<T>(Expression<Func<T>> expression)
    {
        var body = expression.Body as MemberExpression ??
            throw new InvalidOperationException("Expression must be a member access");
        return body.Member as FieldInfo ?? throw new InvalidOperationException("Member must be a field");
    }
    public static PropertyInfo PropertyOf<T>(Expression<Func<T>> expression)
    {
        var body = expression.Body as MemberExpression ??
            throw new InvalidOperationException("Expression must be a member access");
        return body.Member as PropertyInfo ?? throw new InvalidOperationException("Member must be a property");
    }

    public static MethodInfo MethodOf<T>(Expression<Func<T>> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException("Expression must be a method call");
        return body.Method;
    }

    public static MethodInfo MethodOf<TThis, T>(Expression<Func<TThis, T>> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException("Expression must be a method call");
        return body.Method;
    }

    public static MethodInfo MethodOf(Expression<Action> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException("Expression must be a method call");
        return body.Method;
    }
    public static MethodInfo MethodOf<TThis>(Expression<Action<TThis>> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException("Expression must be a method call");
        return body.Method;
    }

    public static T CallGenericMethod<T>(MethodInfo genericMethod, Type[] genericArgs, object? thisArg, object?[] args)
    {
        var m = genericMethod.MakeGenericMethod(genericArgs);
        return (T)m.Invoke(thisArg, args);
    }

    public static void CallGenericMethod(MethodInfo genericMethod, Type[] genericArgs, object? thisArg, object?[] args)
    {
        var m = genericMethod.MakeGenericMethod(genericArgs);
        m.Invoke(thisArg, args);
    }

}
