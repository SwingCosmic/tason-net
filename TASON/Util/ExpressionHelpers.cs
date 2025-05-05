using System.Linq.Expressions;
using System.Reflection;
using static TASON.Util.ReflectionHelpers;

namespace TASON.Util;

/// <summary>
/// 提供工具方法在编译时获取类成员的信息，包括字段、属性和方法
/// </summary>
public static class ExpressionHelpers
{
    private const string NotFieldMember = "Member must be a field";
    private const string NotPropertyMember = "Member must be a property";
    private const string NotMemberAccess = "Expression must be a member access";
    private const string NotMethodCall = "Expression must be a method call";

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static FieldInfo FieldOf<T>(Expression<Func<T>> expression)
    {
        var body = expression.Body as MemberExpression ??
            throw new InvalidOperationException(NotMemberAccess);
        return body.Member as FieldInfo ?? throw new InvalidOperationException(NotFieldMember);
    }

    public static FieldInfo FieldOf<TThis, T>(Expression<Func<TThis, T>> expression)
    {
        var body = expression.Body as MemberExpression ??
            throw new InvalidOperationException(NotMemberAccess);
        return body.Member as FieldInfo ?? throw new InvalidOperationException(NotFieldMember);
    }

    public static PropertyInfo PropertyOf<T>(Expression<Func<T>> expression)
    {
        var body = expression.Body as MemberExpression ??
            throw new InvalidOperationException(NotMemberAccess);
        return body.Member as PropertyInfo ?? throw new InvalidOperationException(NotPropertyMember);
    }

    public static PropertyInfo PropertyOf<TThis, T>(Expression<Func<TThis, T>> expression)
    {
        var body = expression.Body as MemberExpression ??
            throw new InvalidOperationException(NotMemberAccess);
        return body.Member as PropertyInfo ?? throw new InvalidOperationException(NotPropertyMember);
    }

    public static MethodInfo MethodOf<T>(Expression<Func<T>> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException(NotMethodCall);
        return body.Method;
    }

    public static MethodInfo MethodOf(Expression<Action> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException(NotMethodCall);
        return body.Method;
    }

    public static MethodInfo MethodOf<TThis, T>(Expression<Func<TThis, T>> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException(NotMethodCall);
        return body.Method;
    }

    public static MethodInfo MethodOf<TThis>(Expression<Action<TThis>> expression)
    {
        var body = expression.Body as MethodCallExpression ??
            throw new InvalidOperationException(NotMethodCall);
        return body.Method;
    }

#pragma warning restore CS1591
}