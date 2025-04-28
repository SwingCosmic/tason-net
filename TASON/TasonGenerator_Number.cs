

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TASON.Types;

namespace TASON;

public partial class TasonGenerator
{
    bool TryWriteNumberValue(object value, ValueScope scope = ValueScope.Normal)
    {
        return value switch 
        {
            byte u8 => SafeNumber(u8, NumberTypes.UInt8, nameof(NumberTypes.UInt8), scope),
            sbyte i8 => SafeNumber(i8, NumberTypes.Int8, nameof(NumberTypes.Int8), scope),
            ushort u16 => SafeNumber(u16, NumberTypes.UInt16, nameof(NumberTypes.UInt16), scope),
            short i16 => SafeNumber(i16, NumberTypes.Int16, nameof(NumberTypes.Int16), scope),
            char c => SafeNumber(c, NumberTypes.Char, nameof(NumberTypes.Char), scope),
            uint u32 => SafeNumber(u32, NumberTypes.UInt32, nameof(NumberTypes.UInt32), scope),
            int i32 => SafeNumber(i32, NumberTypes.Int32, nameof(NumberTypes.Int32), scope),
            Half f16 => SafeNumber(f16, NumberTypes.Float16, nameof(NumberTypes.Float16), scope),
            float f32 => SafeNumber(f32, NumberTypes.Float32, nameof(NumberTypes.Float32), scope), 
            double f64 => SafeNumber(f64, NumberTypes.Float64, nameof(NumberTypes.Float64), scope),

            ulong u64 => LargeNumber(u64, NumberTypes.UInt64, nameof(NumberTypes.UInt64), scope),
            long i64 => LargeNumber(i64, NumberTypes.Int64, nameof(NumberTypes.Int64), scope),
#if NET7_0_OR_GREATER
            UInt128 u128 => LargeNumber(u128, NumberTypes.UInt128, nameof(NumberTypes.UInt128), scope),
            Int128 i128 => LargeNumber(i128, NumberTypes.Int128, nameof(NumberTypes.Int128), scope),
#endif
            decimal d => LargeNumber(d, NumberTypes.Decimal128, nameof(NumberTypes.Decimal128), scope),
            BigInteger bi => LargeNumber(bi, NumberTypes.BigInt, nameof(NumberTypes.BigInt), scope),

            IntPtr or UIntPtr => UnsafeNumberType(value, scope),
#if NET6_0_OR_GREATER
            NFloat nf =>
    #if NET7_0_OR_GREATER
                NFloat.Size == 8
    #else
                Unsafe.SizeOf<NFloat>() == 8
    #endif
                ? SafeNumber(nf.Value, NumberTypes.Float64, nameof(NumberTypes.Float64), scope)
                : SafeNumber((float)nf.Value, NumberTypes.Float32, nameof(NumberTypes.Float32), scope),
#endif

            _ => false,
        };
    }

    bool UnsafeNumberType(object value, ValueScope scope) 
    {
        if (!options.AllowUnsafeTypes)
        {
            throw new InvalidOperationException($"Cannot serialize type {value.GetType().Name}");
        }

        return value switch
        {
            IntPtr ip => IntPtr.Size == 8
                ? LargeNumber(ip.ToInt64(), NumberTypes.Int64, nameof(NumberTypes.Int64), scope)
                : SafeNumber(ip.ToInt32(), NumberTypes.Int32, nameof(NumberTypes.Int32), scope),
            UIntPtr uip => UIntPtr.Size == 8
                ? LargeNumber(uip.ToUInt64(), NumberTypes.UInt64, nameof(NumberTypes.UInt64), scope)
                : SafeNumber(uip.ToUInt32(), NumberTypes.UInt32, nameof(NumberTypes.UInt32), scope),
            _ => throw new NotImplementedException(),
        };

    }

    bool SafeNumber<T>(T value, TasonNumberScalar<T> type, string name, ValueScope scope)
        where T : struct, 
#if NET7_0_OR_GREATER
        INumber<T>,
#endif
        IEquatable<T>    
    {
        if (options.BuiltinNumberHandling == BuiltinNumberOption.All)
            TypeInstanceValue(value, type, name);
        else if (options.BuiltinNumberHandling == BuiltinNumberOption.ObjectTypeProperty)
        {
            if (scope == ValueScope.ObjectValue)
                TypeInstanceValue(value, type, name);
            else 
                writer.Write(value.ToString()!);
        } 
        else 
            writer.Write(value.ToString()!);

        return true;
    }

    bool LargeNumber<T>(T value, TasonNumberScalar<T> type, string name, ValueScope scope) 
        where T : struct, 
#if NET7_0_OR_GREATER
        INumber<T>,
#endif
        IEquatable<T>
    {
        if (options.BuiltinNumberHandling == BuiltinNumberOption.None) 
            writer.Write(value.ToString()!);
        else if (options.BuiltinNumberHandling == BuiltinNumberOption.ObjectTypeProperty) 
        {
            if (scope == ValueScope.ObjectValue) 
                TypeInstanceValue(value, type, name);
            else 
                writer.Write(value.ToString()!);
        } 
        else
            TypeInstanceValue(value, type, name);
        
        return true;
    } 


}