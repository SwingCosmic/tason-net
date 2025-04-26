using System.Buffers;
using System.Globalization;
using System.Runtime.InteropServices;
using TASON.Serialization;

namespace TASON.Types;

public static class DateTypes
{
    /// <summary>
    /// 标准时间日期格式
    /// </summary>
    public const string StandardFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    
    /// <summary>
    /// 标准日期格式
    /// </summary>
    public const string StandardDateFormat = "yyyy-MM-dd";

    /// <summary>
    /// 标准时间格式
    /// </summary>
    public const string StandardTimeFormat = "HH:mm:ss.fff";

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static DateType Date { get; } = new();
    public static DateTimeOffsetType DateTimeOffset { get; } = new();
    public static TimestampType Timestamp { get; } = new();
    public static DateOnlyType DateOnly { get; } = new();
    public static TimeOnlyType TimeOnly { get; } = new();


    internal static readonly Dictionary<string, ITasonTypeInfo> Types = new()
    {
        [nameof(Date)] = Date,
        [nameof(Timestamp)] = Timestamp,
        [nameof(DateOnly)] = DateOnly,
        [nameof(TimeOnly)] = TimeOnly,
    };
#pragma warning restore CS1591

}

/// <summary>
/// TASON Date类型
/// </summary>
public class DateType : TasonScalarTypeBase<DateTime>
{

    /// <inheritdoc/>
    protected override DateTime DeserializeCore(string text, TasonSerializerOptions options)
    {
        return DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(DateTime value, TasonSerializerOptions options)
    {
        return value.ToUniversalTime().ToString(DateTypes.StandardFormat, CultureInfo.InvariantCulture);
    }
}

/// <summary>
/// 鸭子类型，将<see cref="DateTimeOffset"/>也视为TASON Timestamp类型
/// </summary>
public class DateTimeOffsetType : TasonScalarTypeBase<DateTimeOffset>
{
    /// <inheritdoc/>
    protected override DateTimeOffset DeserializeCore(string text, TasonSerializerOptions options)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(text, CultureInfo.InvariantCulture));
    }

    /// <inheritdoc/>
    protected override string SerializeCore(DateTimeOffset value, TasonSerializerOptions options)
    {
        return value.ToUnixTimeMilliseconds().ToString();
    }
}

/// <summary>
/// TASON Timestamp类型
/// </summary>
public class TimestampType : TasonScalarTypeBase<Timestamp>
{
    /// <inheritdoc/>
    protected override Timestamp DeserializeCore(string text, TasonSerializerOptions options)
    {
        return long.Parse(text, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(Timestamp value, TasonSerializerOptions options)
    {
        return value.Milliseconds.ToString();
    }
}

/// <summary>
/// TASON DateOnly类型
/// </summary>
public class DateOnlyType : TasonScalarTypeBase<DateOnly>
{
    /// <inheritdoc/>
    protected override DateOnly DeserializeCore(string text, TasonSerializerOptions options)
    {
        return DateOnly.Parse(text, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(DateOnly value, TasonSerializerOptions options)
    {
        return value.ToString(DateTypes.StandardDateFormat);
    }
}

/// <summary>
/// TASON TimeOnly类型
/// </summary>
public class TimeOnlyType : TasonScalarTypeBase<TimeOnly>
{
    /// <inheritdoc/>
    protected override TimeOnly DeserializeCore(string text, TasonSerializerOptions options)
    {
        return TimeOnly.Parse(text, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc/>
    protected override string SerializeCore(TimeOnly value, TasonSerializerOptions options)
    {
        return value.ToString(DateTypes.StandardTimeFormat);
    }
}