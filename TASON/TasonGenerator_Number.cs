

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TASON.Types;

namespace TASON;

public partial class TasonGenerator
{
    bool TryGetNumberValue(object value, [NotNullWhen(true)] out string? result)
    {
        result = value switch 
        {
            byte u8 => SafeNumber(u8, NumberTypes.UInt8, nameof(NumberTypes.UInt8)),
            sbyte i8 => SafeNumber(i8, NumberTypes.Int8, nameof(NumberTypes.Int8)),
            ushort u16 => SafeNumber(u16, NumberTypes.UInt16, nameof(NumberTypes.UInt16)),
            short i16 => SafeNumber(i16, NumberTypes.Int16, nameof(NumberTypes.Int16)),
            char c => SafeNumber(c, NumberTypes.Char, nameof(NumberTypes.Char)),
            uint u32 => SafeNumber(u32, NumberTypes.UInt32, nameof(NumberTypes.UInt32)),
            int i32 => SafeNumber(i32, NumberTypes.Int32, nameof(NumberTypes.Int32)),
            Half f16 => SafeNumber(f16, NumberTypes.Float16, nameof(NumberTypes.Float16)),
            float f32 => SafeNumber(f32, NumberTypes.Float32, nameof(NumberTypes.Float32)), 
            double f64 => SafeNumber(f64, NumberTypes.Float64, nameof(NumberTypes.Float64)),

            ulong u64 => LargeNumber(u64, NumberTypes.UInt64, nameof(NumberTypes.UInt64)),
            long i64 => LargeNumber(i64, NumberTypes.Int64, nameof(NumberTypes.Int64)),
#if NET7_0_OR_GREATER
            UInt128 u128 => LargeNumber(u128, NumberTypes.UInt128, nameof(NumberTypes.UInt128)),
            Int128 i128 => LargeNumber(i128, NumberTypes.Int128, nameof(NumberTypes.Int128)),
#endif
            decimal d => LargeNumber(d, NumberTypes.Decimal128, nameof(NumberTypes.Decimal128)),
            BigInteger bi => LargeNumber(bi, NumberTypes.BigInt, nameof(NumberTypes.BigInt)),

            IntPtr ip => IntPtr.Size == 8 
                ? LargeNumber(ip.ToInt64(), NumberTypes.Int64, nameof(NumberTypes.Int64)) 
                : SafeNumber(ip.ToInt32(), NumberTypes.Int32, nameof(NumberTypes.Int32)),
            UIntPtr uip => UIntPtr.Size == 8 
                ? LargeNumber(uip.ToUInt64(), NumberTypes.UInt64, nameof(NumberTypes.UInt64)) 
                : SafeNumber(uip.ToUInt32(), NumberTypes.UInt32, nameof(NumberTypes.UInt32)),
#if NET6_0_OR_GREATER
            NFloat nf =>
#if NET7_0_OR_GREATER
                NFloat.Size == 8
#else
                Unsafe.SizeOf<NFloat>() == 8
#endif
                ? SafeNumber(nf.Value, NumberTypes.Float64, nameof(NumberTypes.Float64))
                : SafeNumber((float)nf.Value, NumberTypes.Float32, nameof(NumberTypes.Float32)),
#endif

            _ => null,
        };
        return result is not null;
    }

    string SafeNumber<T>(T value, TasonNumberScalar<T> type, string name, bool inObject = false)
        where T : struct, 
#if NET7_0_OR_GREATER
        INumber<T>,
#endif
        IEquatable<T>    
    {
        return options.UseBuiltinNumber switch
        {
            BuiltinNumberOption.All => TypeInstanceValue(value, type, name),
            BuiltinNumberOption.ObjectTypeProperty => inObject ? TypeInstanceValue(value, type, name) : value.ToString()!,
            _ => value.ToString()!,
        };
    }

    string LargeNumber<T>(T value, TasonNumberScalar<T> type, string name, bool inObject = false) 
        where T : struct, 
#if NET7_0_OR_GREATER
        INumber<T>,
#endif
        IEquatable<T>
    {
        return options.UseBuiltinNumber switch
        {
            BuiltinNumberOption.None => value.ToString()!,
            BuiltinNumberOption.ObjectTypeProperty => inObject ? TypeInstanceValue(value, type, name) : value.ToString()!,
            _ => TypeInstanceValue(value, type, name),
        };
    } 


}