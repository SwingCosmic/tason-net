using System.Globalization;
using System.Numerics;

namespace TASON.Types;
/// <summary>
/// 实现了<see cref="INumber{T}"/>的数字标量类型实现
/// </summary>
/// <typeparam name="T">对应的数字类型</typeparam>
public abstract class TasonNumberScalar<T> : ITasonScalarType
    where T : struct, IEquatable<T>, INumber<T>
{
    /// <inheritdoc/>
    public TasonTypeInstanceKind Kind { get; }
    /// <inheritdoc/>
    public Type Type { get; }

    public TasonNumberScalar()
    {
        Type = typeof(T);
        Kind = TasonTypeInstanceKind.Scalar;
    }

    /// <inheritdoc/>
    public object Deserialize(string text, SerializerOptions options)
    {
        var (num, radix, isNegative) = PrimitiveHelpers.ParseBuiltinNumber(text);
        return ParseValue(num, radix, isNegative);
    }

    protected abstract T ParseValue(string text, int radix, bool isNegative);

    /// <inheritdoc/>
    public string Serialize(object value, SerializerOptions options)
    {
        var s = value.ToString() ?? "";
        return s.Replace(PrimitiveHelpers.InfinitySymbol, PrimitiveHelpers.Infinity);
    }

    protected static string AddNegative(string text, bool isNegative)
    {
        return isNegative ? "-" + text : text;
    }
}

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
public class UInt8Type : TasonNumberScalar<byte>

{
    protected override byte ParseValue(string text, int radix, bool isNegative)
    {
        if (isNegative)
        {
            throw new ArgumentException("UInt8 cannot be negative");
        }
        else
        {
            return Convert.ToByte(text, radix);
        }
    }
}

public class Int16Type : TasonNumberScalar<short>
{
    protected override short ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10 && isNegative)
        {
            return (short)(-1 * Convert.ToInt16(text, radix));
        }
        else
        {
            return Convert.ToInt16(AddNegative(text, isNegative), radix);
        }
    }
}

public class Int32Type : TasonNumberScalar<int>
{
    protected override int ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10 && isNegative)
        {
            return -1 * Convert.ToInt32(text, radix);
        }
        else
        {
            return Convert.ToInt32(AddNegative(text, isNegative), radix);
        }
    }
}

public class Int64Type : TasonNumberScalar<long>
{
    protected override long ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10 && isNegative)
        {
            return -1L * Convert.ToInt64(text, radix);
        }
        else
        {
            return Convert.ToInt64(AddNegative(text, isNegative), radix);
        }
    }
}

public class Float32Type : TasonNumberScalar<float>
{
    protected override float ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10)
        {
            throw new ArgumentException("Float32 only supports 10-based number");
        }
        return float.Parse(text, CultureInfo.InvariantCulture);
    }
}

public class Float64Type : TasonNumberScalar<double>
{
    protected override double ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10)
        {
            throw new ArgumentException("Float64 only supports 10-based number");
        }
        return double.Parse(text, CultureInfo.InvariantCulture);
    }
}

public class Decimal128Type : TasonNumberScalar<decimal>
{
    protected override decimal ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10)
        {
            throw new ArgumentException("Decimal128 only supports 10-based number");
        }
        return decimal.Parse(text, CultureInfo.InvariantCulture);
    }
}

public class BigIntType : TasonNumberScalar<BigInteger>
{
    protected override BigInteger ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10)
        {
            BigInteger sign = isNegative ? -1 : 1;
            return radix switch
            {
                2 => sign * BigInteger.Parse(text, NumberStyles.BinaryNumber),
                8 => sign * ParseOctalNumber(text),
                16 => sign * BigInteger.Parse(text, NumberStyles.HexNumber),
                _ => throw new FormatException($"Invalid radix '{radix}'"),
            };
        }
        return BigInteger.Parse(AddNegative(text, isNegative), CultureInfo.InvariantCulture);
    }

    static BigInteger ParseOctalNumber(string oct)
    {
        return oct.Aggregate(new BigInteger(), (b, c) => b * 8 + (c - '0'));
    }
}

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释