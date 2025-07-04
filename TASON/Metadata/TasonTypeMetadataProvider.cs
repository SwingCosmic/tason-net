using System.Runtime.CompilerServices;

namespace TASON.Metadata;

/// <summary>
/// 提供指定类型的元数据
/// </summary>
public static class TasonTypeMetadataProvider 
{
    static readonly ConditionalWeakTable<Type, ITasonTypeMetadata> cache = new();
    static readonly ConditionalWeakTable<Type, ITasonEnumMetadata> enumCache = new();

    /// <summary>
    /// 获取<paramref name="type"/>对应的默认<see cref="ITasonTypeMetadata"/>
    /// </summary>
    /// <param name="type">要获取的类型</param>
    /// <returns>默认<see cref="ITasonTypeMetadata"/>实现</returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="type"/>是抽象类，委托，或者类和结构以外的其它类型
    /// </exception>
    public static ITasonTypeMetadata GetMetadata(Type type)
    {
        return cache.GetValue(type, t => new TasonClassMetadata(t));
    }

    /// <summary>
    /// 获取<typeparamref name="T"/>对应的默认<see cref="ITasonTypeMetadata"/>
    /// </summary>
    /// <typeparam name="T">要获取的类型</typeparam>
    /// <returns>默认<see cref="ITasonTypeMetadata"/>实现</returns>
    public static ITasonTypeMetadata GetMetadata<T>() where T : notnull
    {
        return GetMetadata(typeof(T));
    }

    internal static void SetMetadata(Type type, ITasonTypeMetadata meta)
    {
        cache.AddOrUpdate(type, meta);
    }

    public static ITasonEnumMetadata GetEnumMetadata(Type type)
    {
        return enumCache.GetValue(type, CreateEnumMetadata);
    }

    static readonly Type enumClassType = typeof(TasonEnumMetadata<,>);
    static ITasonEnumMetadata CreateEnumMetadata(Type type)
    {
        if (!type.IsEnum)
            throw new InvalidOperationException();

        var enumType = type.GetEnumUnderlyingType();
        var cls = enumClassType.MakeGenericType(type, enumType);
        return (Activator.CreateInstance(cls) as ITasonEnumMetadata)!;
    }
}