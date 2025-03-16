using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.Cryptography;

namespace TASON;

/// <summary>
/// 包含一些基础类型的工具方法
/// </summary>
public static class PrimitiveHelpers
{
    private const string NaN = "NaN";
    private const string Infinity = "Infinity";

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
}
