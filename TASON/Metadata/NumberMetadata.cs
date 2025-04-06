using System.Numerics;
using TASON.Types;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TASON.Metadata;

internal static class NumberMetadata
{

    static readonly Dictionary<Type, ITasonScalarType> ClrTypeMapping = new()
    {
        [typeof(sbyte)] = NumberTypes.Int8,
        [typeof(byte)] = NumberTypes.UInt8,
        [typeof(short)] = NumberTypes.Int16,
        [typeof(ushort)] = NumberTypes.UInt16,
        [typeof(char)] = NumberTypes.Char,
        [typeof(int)] = NumberTypes.Int32,
        [typeof(uint)] = NumberTypes.UInt32,
        [typeof(long)] = NumberTypes.Int64,
        [typeof(ulong)] = NumberTypes.UInt64,
#if NET7_0_OR_GREATER
        [typeof(Int128)] = NumberTypes.Int128,
        [typeof(UInt128)] = NumberTypes.UInt128,
#endif

        [typeof(Half)] = NumberTypes.Float16,
        [typeof(float)] = NumberTypes.Float32,
        [typeof(double)] = NumberTypes.Float64,
        [typeof(decimal)] = NumberTypes.Decimal128,
        [typeof(BigInteger)] = NumberTypes.BigInt,

#if NET6_0_OR_GREATER
        [typeof(NFloat)] =
    #if NET7_0_OR_GREATER
            NFloat.Size == 8
    #else
            Unsafe.SizeOf<NFloat>() == 8
    #endif
                ? NumberTypes.Float64 : NumberTypes.Float32,
#endif
        [typeof(IntPtr)] = IntPtr.Size == 8 ? NumberTypes.Int64 : NumberTypes.Int32,
        [typeof(UIntPtr)] = UIntPtr.Size == 8 ? NumberTypes.UInt64 : NumberTypes.UInt32,
    };

    public static bool TryGetClrType(Type type, out ITasonScalarType scalarType)
    {
        return ClrTypeMapping.TryGetValue(type, out scalarType);
    }

    public static T Deserialize<T>(string number, SerializerOptions options)
        where T : struct,
#if NET7_0_OR_GREATER
        INumber<T>,
#endif
        IEquatable<T>
    {
        var typeInfo = (TasonNumberScalar<T>)ClrTypeMapping[typeof(T)];
        return typeInfo.DeserializeInternal(number, options);
    }
}