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
public class Int8Type : TasonNumberScalar<sbyte>

{
    protected override sbyte ParseValue(string text, int radix, bool isNegative)
    {
        if (radix != 10 && isNegative)
        {
            return (sbyte)(-1 * Convert.ToSByte(text, radix));
        }
        else
        {
            return Convert.ToSByte(AddNegative(text, isNegative), radix);
        }
    }
}

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

public class UInt16Type : TasonNumberScalar<ushort>
{
    protected override ushort ParseValue(string text, int radix, bool isNegative)
    {
        if (isNegative)
        {
            throw new ArgumentException("UInt16 cannot be negative");
        }
        else
        {
            return Convert.ToUInt16(text, radix);
        }
    }
}

public class CharType : TasonNumberScalar<char>
{
    protected override char ParseValue(string text, int radix, bool isNegative)
    {
        if (isNegative)
        {
            throw new ArgumentException("Char cannot be negative");
        }
        else
        {
            return (char)Convert.ToUInt16(text, radix);
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

public class UInt32Type : TasonNumberScalar<uint>
{
    protected override uint ParseValue(string text, int radix, bool isNegative)
    {
        if (isNegative)
        {
            throw new ArgumentException("UInt32 cannot be negative");
        }
        else
        {
            return Convert.ToUInt32(text, radix);
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

public class UInt64Type : TasonNumberScalar<ulong>
{
    protected override ulong ParseValue(string text, int radix, bool isNegative)
    {
        if (isNegative)
        {
            throw new ArgumentException("UInt64 cannot be negative");
        }
        else
        {
            return Convert.ToUInt64(text, radix);
        }
    }
}

#if NET7_0_OR_GREATER
public class Int128Type : TasonNumberScalar<Int128>
{
    protected override Int128 ParseValue(string text, int radix, bool isNegative)
    {
        return Int128.Parse(BigIntType.ParseBigInteger(text, radix, isNegative).ToString());
    }
}

public class UInt128Type : TasonNumberScalar<UInt128>
{
    protected override UInt128 ParseValue(string text, int radix, bool isNegative)
    {
        if (isNegative)
        {
            throw new ArgumentException("UInt128 cannot be negative");
        }
        else
        {
            return UInt128.Parse(BigIntType.ParseBigInteger(text, radix, false).ToString());
        }
    }
}
#endif

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
    internal static BigInteger ParseBigInteger(string text, int radix, bool isNegative)
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
    protected override BigInteger ParseValue(string text, int radix, bool isNegative)
    {
        return ParseBigInteger(text, radix, isNegative);
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