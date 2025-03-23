using System.Globalization;
using System.Numerics;

namespace TASON.Types;
/// <summary>
/// 实现了<see cref="INumber{T}"/>的数字标量类型实现
/// </summary>
/// <typeparam name="T">对应的数字类型</typeparam>
public abstract class TasonNumberScalar<T> : TasonScalarTypeBase<T>
    where T : struct, 
#if NET7_0_OR_GREATER
    INumber<T>,
#endif
    IEquatable<T>
{

    /// <inheritdoc/>
    protected override T DeserializeCore(string text, SerializerOptions options)
    {
        var (num, radix, isNegative) = PrimitiveHelpers.ParseBuiltinNumber(text);
        return ParseValue(num, radix, isNegative);
    }

    protected abstract T ParseValue(string text, int radix, bool isNegative);

    /// <inheritdoc/>
    protected override string SerializeCore(T value, SerializerOptions options)
    {
        return value.ToString()!;
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

public class Float16Type : TasonNumberScalar<Half>
{
    protected override Half ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10)
        {
            throw new ArgumentException("Float16 only supports 10-based number");
        }
        return Half.Parse(AddNegative(text, isNegative), CultureInfo.InvariantCulture);
    }

    protected override string SerializeCore(Half value, SerializerOptions options)
    {
        var s = value.ToString()!;
        return s.Replace(PrimitiveHelpers.InfinitySymbol, PrimitiveHelpers.Infinity);
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
        return float.Parse(AddNegative(text, isNegative), CultureInfo.InvariantCulture);
    }

    protected override string SerializeCore(float value, SerializerOptions options)
    {
        var s = value.ToString()!;
        return s.Replace(PrimitiveHelpers.InfinitySymbol, PrimitiveHelpers.Infinity);
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
        return double.Parse(AddNegative(text, isNegative), CultureInfo.InvariantCulture);
    }

    protected override string SerializeCore(double value, SerializerOptions options)
    {
        var s = value.ToString()!;
        return s.Replace(PrimitiveHelpers.InfinitySymbol, PrimitiveHelpers.Infinity);
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
        return decimal.Parse(AddNegative(text, isNegative), CultureInfo.InvariantCulture);
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
#if NET8_0_OR_GREATER
                2 => sign * BigInteger.Parse(text, NumberStyles.BinaryNumber),
#else
                2 => sign * ParseBinaryNumber(text),
#endif
                8 => sign * ParseOctalNumber(text),
                16 => sign * BigInteger.Parse(text, NumberStyles.HexNumber),
                _ => throw new FormatException($"Invalid radix '{radix}'"),
            };
        }
        return BigInteger.Parse(AddNegative(text, isNegative), CultureInfo.InvariantCulture);
    }

#if !NET8_0_OR_GREATER
    static BigInteger ParseBinaryNumber(string bin)
    {
        if (bin.Any(c => c != '0' && c != '1'))
        {
            throw new FormatException("The value could not be parsed.");
        }
        return bin.Aggregate(new BigInteger(0), (b, c) => b * 2 + (c - '0'));
    }
#endif
    static BigInteger ParseOctalNumber(string oct)
    {
        if (oct.Any(c => c is < '0' or > '7'))
        {
            // 和BigInteger.Parse方法提供一致的错误信息
            throw new FormatException("The value could not be parsed.");
        }
        return oct.Aggregate(new BigInteger(), (b, c) => b * 8 + (c - '0'));
    }
}

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释