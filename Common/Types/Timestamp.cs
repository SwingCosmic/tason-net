using System.Numerics;
using System.Runtime.InteropServices;

namespace TASON.Types;

/// <summary>
/// 表示一个Unix时间戳（单位为毫秒）
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Timestamp : IEquatable<Timestamp>
#if NET7_0_OR_GREATER
    , IEqualityOperators<Timestamp, Timestamp, bool>
    , IComparisonOperators<Timestamp, Timestamp, bool>
    , IAdditionOperators<Timestamp, Timestamp, Timestamp>
    , ISubtractionOperators<Timestamp, Timestamp, Timestamp>
    , IAdditionOperators<Timestamp, long, Timestamp>
    , ISubtractionOperators<Timestamp, long, Timestamp>
#endif
{
    /// <summary>毫秒时间戳的值，即自 1970-01-01T00:00:00.000Z 起已经过的毫秒数。</summary>
    public readonly long Milliseconds = 0;

    /// <summary>使用指定毫秒时间戳的数值来初始化<see cref="Timestamp"/>结构的新实例</summary>
    /// <param name="timestamp">毫秒时间戳</param>
    public Timestamp(long timestamp)
    {
        Milliseconds = timestamp;
    }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static implicit operator Timestamp(long timestamp)
    {
        return new Timestamp(timestamp);
    }

    // 存在轻微的精度损失，所以为explicit转换
    public static explicit operator Timestamp(DateTimeOffset timestamp)
    {
        return new Timestamp(timestamp.ToUnixTimeMilliseconds());
    }
    
    public static implicit operator DateTime(Timestamp timestamp)
    {
        return ((DateTimeOffset)timestamp).UtcDateTime;
    }    
    
    public static implicit operator DateTimeOffset(Timestamp timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp.Milliseconds);
    }

    public static bool operator <(Timestamp left, Timestamp right) => left.Milliseconds < right.Milliseconds;

    public static bool operator >(Timestamp left, Timestamp right) => left.Milliseconds > right.Milliseconds;

    public static bool operator <=(Timestamp left, Timestamp right) => left.Milliseconds <= right.Milliseconds;

    public static bool operator >=(Timestamp left, Timestamp right) => left.Milliseconds >= right.Milliseconds;

    public static Timestamp operator +(Timestamp left, Timestamp right) => left.Milliseconds + right.Milliseconds;

    public static Timestamp operator -(Timestamp left, Timestamp right) => left.Milliseconds - right.Milliseconds;

    public static Timestamp operator +(Timestamp left, long offset) => left.Milliseconds + offset;

    public static Timestamp operator -(Timestamp left, long offset) => left.Milliseconds - offset;
#pragma warning restore CS1591

    /// <summary>
    /// 获取一个<see cref="Timestamp"/>对象，其日期和时间设置为当前的协调世界时 (UTC) 日期和时间，其偏移量为 0。
    /// </summary>
    /// <returns>表示当前毫秒时间戳的<see cref="Timestamp"/>对象</returns>
    public static Timestamp UtcNow()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 将<see cref="Timestamp"/>实例转换成<see cref="DateTimeOffset"/>
    /// </summary>
    public DateTimeOffset ToDateTimeOffset()
    {
        return (DateTimeOffset)this;
    }

    /// <summary>
    /// 返回以秒为单位的时间戳，通过截去毫秒部分。
    /// </summary>
    /// <returns>以秒为单位的时间戳</returns>
    public long Seconds => Milliseconds / 1000;
}