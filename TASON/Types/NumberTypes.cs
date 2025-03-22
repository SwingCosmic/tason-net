
using System;
using System.Numerics;

namespace TASON.Types;

/// <summary>
/// 内置数值类型
/// </summary>
public static class NumberTypes
{

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static UInt8Type UInt8 { get; } = new();
    public static Int16Type Int16 { get; } = new();
    public static Int32Type Int32 { get; } = new();
    public static Int64Type Int64 { get; } = new();
    public static Float32Type Float32 { get; } = new();
    public static Float64Type Float64 { get; } = new();
    public static Decimal128Type Decimal128 { get; } = new();
    public static BigIntType BigInt { get; } = new();


    internal static readonly Dictionary<string, ITasonTypeInfo> Types = new()
    {
        [nameof(UInt8)] = UInt8,
        [nameof(Int16)] = Int16,
        [nameof(Int32)] = Int32,
        [nameof(Int64)] = Int64,
        [nameof(Float32)] = Float32,
        [nameof(Float64)] = Float64,
        [nameof(Decimal128)] = Decimal128,
        [nameof(BigInt)] = BigInt,
    };
#pragma warning restore CS1591

}
