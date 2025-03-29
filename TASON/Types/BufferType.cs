using System.Numerics;
using TASON.Serialization;
using TASON.Util;

namespace TASON.Types;

/// <summary>
/// Buffer序列化所用的数据格式
/// </summary>
public enum BufferDataFormat
{
    /// <summary>Base64字符串</summary>
    Base64,
    /// <summary>16进制字符串</summary>
    Hex,
}


/// <summary>
/// 表示一段以字节为单位的二进制数据
/// </summary>
public sealed class Buffer : IEquatable<Buffer>
#if NET7_0_OR_GREATER
    , IEqualityOperators<Buffer, Buffer, bool>
#endif
{
    /// <summary>
    /// 指定序列化所用的数据格式
    /// </summary>
    public BufferDataFormat Type { get; }

    public const string Base64 = "base64";
    public const string Hex = "hex";

    internal byte[] data = [];
    private Buffer(BufferDataFormat type)
    {
        Type = type;
    }

    public Buffer(BufferDataFormat type, byte[] data) : this(type)
    {
        this.data = data;
    }  
    
    public Buffer(BufferDataFormat type, ReadOnlySpan<byte> data) : this(type)
    {
        this.data = data.ToArray();
    } 
    
    public Buffer(string dataString)
    {
        var index = dataString.IndexOf(',');
        if (index < 0)
        {
            throw new FormatException($"Invalid data string: '{dataString[..20]} ...'");
        }

        var type = dataString[..index].ToLower();
        var data = dataString[(index + 1)..].Trim();

        if (type == Base64)
        {
            Type = BufferDataFormat.Base64;
            this.data = Convert.FromBase64String(data);
        }
        else if (type == Hex)
        {
            Type = BufferDataFormat.Hex;
            this.data = PrimitiveHelpers.FromHexString(data);
        }
        else
        {
            throw new FormatException($"Invalid buffer type: {type}");
        }
    }

    public byte[] Data => data;

    /// <summary>
    /// 将<see cref="Buffer"/>序列化成<see cref="Type"/>指定的字符串格式
    /// </summary>
    /// <returns>序列化后的字符串</returns>
    public override string ToString()
    {
        if (Type == BufferDataFormat.Hex)
        {
            return $"{Hex},{PrimitiveHelpers.ToHexString(data)}";
        } else
        {
            return $"{Base64},{Convert.ToBase64String(data)}";
        }
    }

    /// <inheritdoc/>
    public bool Equals(Buffer? other)
    {
        if (other is null) return false;
        return Type == other.Type && data.AsSpan().SequenceEqual(other.data.AsSpan());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Buffer);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Type, data);
    }

    /// <inheritdoc/>
    public static bool operator ==(Buffer? left, Buffer? right)
    {
        return (left, right) switch 
        {
            (null, null) => true,
            (null, _) or (_, null) => false,
            _ => left.Equals(right),
        };
    }

    /// <inheritdoc/>
    public static bool operator !=(Buffer? left, Buffer? right) => !(left == right);
}

/// <summary>
/// <see cref="Buffer"/>的类型信息
/// </summary>
public class BufferType : TasonScalarTypeBase<Buffer>
{
    /// <inheritdoc/>
    protected override Buffer DeserializeCore(string text, SerializerOptions options)
    {
        return new Buffer(text);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(Buffer value, SerializerOptions options)
    {
        return value.ToString();
    }
}