using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using TASON.Serialization;

namespace TASON.Metadata;

/// <summary>
/// 提供枚举的反射元数据
/// </summary>
public interface ITasonEnumMetadata : ITasonMetadata
{
    /// <summary>
    /// 枚举的实际类型，只能为整数类型
    /// </summary>
    Type UnderlyingType { get; }

    /// <summary>
    /// 枚举的序列化值类型
    /// </summary>
    EnumValueType ValueType { get; }

    /// <summary>
    /// 根据枚举值获取序列化值
    /// </summary>
    /// <param name="enum">枚举值</param>
    /// <returns>序列化值，可能为字符串</returns>
    object GetValue(Enum @enum);

    /// <summary>
    /// 根据序列化值获取枚举值
    /// </summary>
    /// <param name="value">序列化值，可能为字符串</param>
    /// <returns>枚举值</returns>
    /// <exception cref="InvalidCastException">
    /// 对<see cref="ValueType" />为<see cref="EnumValueType.String"/>的枚举提供了非字符串的类型
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// 对具有<see cref="FlagsAttribute"/>的枚举尝试从字符串获枚举值
    /// </exception>
    Enum GetEnum(object value);
}

/// <summary>
/// 枚举的序列化值类型
/// </summary>
public enum EnumValueType
{
    /// <summary>数字</summary>
    Number = 0,
    /// <summary>字符串</summary>
    String = 1,
}

/// <summary>
/// 提供枚举相关的类型元数据
/// </summary>
/// <typeparam name="E">枚举类型</typeparam>
/// <typeparam name="T">枚举实际值的类型</typeparam>
public class TasonEnumMetadata<E, T> : ITasonEnumMetadata
    where E : unmanaged, Enum
    where T : unmanaged
{
    /// <inheritdoc/>
    public Type UnderlyingType { get; }

    /// <inheritdoc/>
    public EnumValueType ValueType { get; }

    /// <inheritdoc/>
    public Type Type { get; }

    readonly Dictionary<E, string> enumToValue = new();
    readonly Dictionary<string, E> valueToEnum = new();

    readonly bool isFlags = false;
    public TasonEnumMetadata()
    {
        var enumType = typeof(E);
        if (!enumType.IsEnum)
            throw new InvalidOperationException($"Type '{enumType.FullName}' is not an enum type");

        Type = enumType;
        UnderlyingType = enumType.GetEnumUnderlyingType();
        if (UnderlyingType != typeof(T))
            throw new InvalidOperationException($"Type '{UnderlyingType.Name}' is not the underlying type of type '{enumType.FullName}'");

        var isStringEnum = enumType.IsDefined(typeof(TasonStringEnumAttribute), true);
        ValueType = isStringEnum ? EnumValueType.String : EnumValueType.Number;

        isFlags = enumType.IsDefined(typeof(FlagsAttribute));

        if (isStringEnum)
        {
            InitStringMap();
        }
    }

    void InitStringMap()
    {

#if NET5_0_OR_GREATER
        var names = Enum.GetNames<E>();
        E[] values = Enum.GetValues<E>();
#else
        var names = Enum.GetNames(typeof(E));
        E[] values = (E[])Enum.GetValues(typeof(E));
#endif
        for (int i = 0; i < values.Length; i++)
        {
            var e = values[i];
            T underlyingValue = Unsafe.As<E, T>(ref e);
            var name = names[i];

            var field = Type.GetField(name)!;
            var attr = field.GetCustomAttribute<TasonEnumValueAttribute>();
            if (attr is not null)
                name = attr.Value;

            enumToValue[e] = name;

            // 遇到重复值会发生覆盖
            valueToEnum[name] = e;
        }
    }
    /// <inheritdoc/>
    public object GetValue(Enum @enum)
    {
        E e = (E)@enum;
        if (ValueType == EnumValueType.String)
            return enumToValue[e];
        return Unsafe.As<E, T>(ref e);
    }
    /// <inheritdoc/>
    public Enum GetEnum(object value)
    {
        if (value is not string && ValueType == EnumValueType.String)
            throw new InvalidCastException("Invalid enum value type");

        if (ValueType == EnumValueType.String)
        {
            if (isFlags)
                throw new NotSupportedException("Converting string to enum with FlagsAttribute is not supported");
            return valueToEnum[(string)value];
        }
        return (Enum)Enum.ToObject(Type, value);
    }
}