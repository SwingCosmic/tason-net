
using System;
using System.Numerics;

namespace TASON.Types;

/// <summary>
/// 内置数值类型
/// </summary>
public static class NumberTypes
{

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static Int8Type Int8 { get; } = new();
    public static UInt8Type UInt8 { get; } = new();
    public static Int16Type Int16 { get; } = new();
    public static UInt16Type UInt16 { get; } = new();
    public static CharType Char { get; } = new();
    public static Int32Type Int32 { get; } = new();
    public static UInt32Type UInt32 { get; } = new();
    public static Int64Type Int64 { get; } = new();
    public static UInt64Type UInt64 { get; } = new();
#if NET7_0_OR_GREATER
    public static Int128Type Int128 { get; } = new();
    public static UInt128Type UInt128 { get; } = new();
#endif
    public static Float16Type Float16 { get; } = new();
    public static Float32Type Float32 { get; } = new();
    public static Float64Type Float64 { get; } = new();
    public static Decimal128Type Decimal128 { get; } = new();
    public static BigIntType BigInt { get; } = new();


    internal static readonly Dictionary<string, ITasonTypeInfo> Types = new()
    {
        [nameof(Int8)] = Int8,
        [nameof(UInt8)] = UInt8,
        [nameof(Int16)] = Int16,
        [nameof(UInt16)] = UInt16,
        [nameof(Int32)] = Int32,
        [nameof(UInt32)] = UInt32,
        [nameof(Int64)] = Int64,
        [nameof(UInt64)] = UInt64,
#if NET7_0_OR_GREATER
        [nameof(Int128)] = Int128,
        [nameof(UInt128)] = UInt128,
#endif
        [nameof(Float16)] = Float16,
        [nameof(Float32)] = Float32,
        [nameof(Float64)] = Float64,
        [nameof(Decimal128)] = Decimal128,
        [nameof(BigInt)] = BigInt,
    };
    
    internal static readonly Dictionary<string, string> Aliases = new()
    {
        ["SByte"] = nameof(Int8),
        ["Byte"] = nameof(UInt8),
        ["Short"] = nameof(Int16),
        ["Int"] = nameof(Int32),
        ["Long"] = nameof(Int64),
        ["Half"] = nameof(Float16),
        ["Single"] = nameof(Float32),
        ["Double"] = nameof(Float64),
        ["Decimal"] = nameof(Decimal128),
        ["BigInteger"] = nameof(BigInt),
    };
#pragma warning restore CS1591

}
