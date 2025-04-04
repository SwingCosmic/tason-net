using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;
using System.Buffers;

namespace TASON.Util;

/// <summary>
/// 包含一些基础类型的工具方法
/// </summary>
public static class PrimitiveHelpers
{
    public const string NaN = "NaN";
    public const string Infinity = "Infinity";
    public const string InfinitySymbol = "∞";

    /// <summary>
    /// 去除字符串中的转义字符
    /// </summary>
    /// <param name="str">待去除的字符串</param>
    /// <returns>去除转义后的字符串</returns>
    /// <exception cref="FormatException">存在非法的转义序列</exception>
    public static string Unescape(this string str)
    {
        var result = new StringBuilder();
        int i = 0;

        while (i < str.Length)
        {
            char c = str[i];
            if (c != '\\')
            {
                result.Append(c);
                i++;
                continue;
            }

            if (++i >= str.Length)
                throw new FormatException("Broken escape sequence");
            c = str[i];

            result.Append(c switch
            {
                'u' => ParseUnicode(str, ref i),
                'x' => ParseHex(str, ref i),
                '"' => '"',
                '\'' => '\'',
                '\\' => '\\',
                't' => '\t',
                'b' => '\b',
                'r' => '\r',
                'n' => '\n',
                'f' => '\f',
                'v' => '\v',
                '0' => '\0',
                _ => throw new FormatException($"Invalid escape sequence '\\{c}'"),
            });
            i++;
        }
        return result.ToString();
    }

    private static char ParseHex(string str, ref int i)
    {
        if (i + 2 >= str.Length)
            throw new FormatException("Broken hex escape sequence");
        string hex = str.Substring(i + 1, 2);
        if (!int.TryParse(hex, NumberStyles.HexNumber, null, out int hexValue))
            throw new FormatException("Invalid hex escape sequence");

        i += 2;
        return (char)hexValue;
    }

    private static char ParseUnicode(string str, ref int i)
    {
        if (i + 4 >= str.Length)
            throw new FormatException("Broken Unicode escape sequence");
        string hex = str.Substring(i + 1, 4);
        if (!int.TryParse(hex, NumberStyles.HexNumber, null, out int unicodeValue))
            throw new FormatException("Invalid Unicode escape sequence");

        i += 4;
        return (char)unicodeValue;
    }

    /// <summary>
    /// Converts the specified string, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer array.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="str"/>.</returns>
    public static byte[] FromHexString(string str)
    {
#if NET5_0_OR_GREATER
        return Convert.FromHexString(str);
#else
        if (str.Length % 2 != 0)
        {
            throw new FormatException("The input is not a valid hex string as its length is not a multiple of 2.");
        }

        var bytes = new byte[str.Length / 2];
        for (int i = 0; i < str.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
        }

        return bytes;
#endif
    }

    /// <summary>
    /// Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with uppercase hex characters.
    /// </summary>
    /// <param name="bytes">An array of 8-bit unsigned integers.</param>
    /// <returns>The string representation in hex of the elements in <paramref name="bytes"/>.</returns>
    public static string ToHexString(byte[] bytes)
    {
#if NET5_0_OR_GREATER
        return Convert.ToHexString(bytes);
#else
        var sb = new StringBuilder(bytes.Length * 2);
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
#endif
    }


    /// <summary>
    /// 解析TASON数字字符串为<see langword="double"/>
    /// </summary>
    /// <param name="input">TASON数字字符串</param>
    /// <returns>解析后的数字</returns>
    /// <exception cref="FormatException">非法的数字格式</exception>
    public static double ParseTasonNumber(string input)
    {
        // 处理特殊值 NaN 和 Infinity
        if (input == NaN || input == '+' + NaN || input == '-' + NaN)
        {
            return double.NaN;
        }
        else if (input == Infinity || input == '+' + Infinity)
        {
            return double.PositiveInfinity;
        }
        else if (input == '-' + Infinity)
        {
            return double.NegativeInfinity;
        }
        else
        {
            // 处理数值部分
            var isNegative = false;
            if (input.StartsWith('-'))
            {
                isNegative = true;
                input = input[1..];
            }
            else if (input.StartsWith('+'))
            {
                input = input[1..];
            }

            // 处理十六进制
            if (input.StartsWith("0x"))
            {
                var hex = input[2..];
                double result = Convert.ToInt64(hex, 16);
                return isNegative ? -result : result;
            }
            // 处理八进制
            else if (input.StartsWith("0o"))
            {
                var oct = input[2..];
                double result = Convert.ToInt64(oct, 8);
                return isNegative ? -result : result;
            }
            // 处理二进制
            else if (input.StartsWith("0b"))
            {
                var bin = input[2..];
                double result = Convert.ToInt64(bin, 2);
                return isNegative ? -result : result;
            }
            // 处理十进制
            else
            {
                try
                {
                    var result = double.Parse(input, CultureInfo.InvariantCulture);
                    return isNegative ? -result : result;
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Invalid number format '{input}'", ex);
                }
            }
        }
    }

    /// <summary>
    /// 解析内置数字类型字符串为数字信息
    /// </summary>
    /// <param name="input">内置数字类型字符串</param>
    /// <returns>解析后的数字信息，包括进制，去除进制前缀的数字，是否为负数</returns>
    public static (string value, int radix, bool isNegative) ParseBuiltinNumber(string input)
    {
        // 处理特殊值 NaN 和 Infinity
        if (input == NaN || input == '+' + NaN || input == '-' + NaN)
        {
            return (NaN, 10, false);
        }
        else if (input == Infinity || input == '+' + Infinity)
        {
            return (InfinitySymbol, 10, false);
        }
        else if (input == '-' + Infinity)
        {
            return ("-" + InfinitySymbol, 10, true);
        }
        else
        {
            // 处理数值部分
            var isNegative = false;
            if (input.StartsWith('-'))
            {
                isNegative = true;
                input = input[1..];
            }
            else if (input.StartsWith('+'))
            {
                input = input[1..];
            }

            // 处理十六进制
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                var hex = input[2..];
                return (hex, 16, isNegative);
            }
            // 处理八进制
            else if (input.StartsWith("0o", StringComparison.OrdinalIgnoreCase))
            {
                var oct = input[2..];
                return (oct, 8, isNegative);
            }
            // 处理二进制
            else if (input.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                var bin = input[2..];
                return (bin, 2, isNegative);
            }
            // 处理十进制
            else
            {
                return (input, 10, isNegative);
            }
        }
    }


    /// <summary>
    /// Determines whether one or more bit fields are set in the current instance.<br/>
    /// 该方法假定枚举类型<typeparamref name="E"/>对应值的数字类型是<typeparamref name="N"/>，
    /// 如果不匹配会产生未定义的行为
    /// </summary>
    /// <param name="flags">具有标志位的枚举值</param>
    /// <param name="value">An enumeration value.</param>
    public static bool HasFlagFast<E, N>(this E flags, E value) 
        where N : unmanaged
#if NET7_0_OR_GREATER
        , IBinaryInteger<N>
#endif
        where E : struct, Enum
    { 
#if NET7_0_OR_GREATER
        N left = Unsafe.As<E, N>(ref flags);
        N right = Unsafe.As<E, N>(ref value);
        return (left & right) == right;
#else
        return flags.HasFlag(value);
#endif
    }    
    
    /// <summary>
    /// Determines whether one or more bit fields are set in the current instance.<br/>
    /// 该方法假定枚举类型<typeparamref name="E"/>对应值的数字类型是<see langword="int"/>，
    /// 如果不匹配会产生未定义的行为
    /// </summary>
    /// <param name="flags">具有标志位的枚举值</param>
    /// <param name="value">An enumeration value.</param>
    public static bool HasFlagFast<E>(this E flags, E value) 
        where E : struct, Enum
    { 
#if NET7_0_OR_GREATER
        int left = Unsafe.As<E, int>(ref flags);
        int right = Unsafe.As<E, int>(ref value);
        return (left & right) == right;
#else
        return flags.HasFlag(value);
#endif
    }
}
