
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;

namespace TASON;

internal enum ValueScope 
{
    Normal,
    ObjectValue,
}

public partial class TasonGenerator
{
    readonly TasonWriter writer;
    readonly SerializerOptions options;
    readonly TasonTypeRegistry registry;
    public TasonGenerator(TasonWriter writer, SerializerOptions options, TasonTypeRegistry registry)
    {
        this.writer = writer;
        this.options = options;
        this.registry = registry;
    }

    public static string ToTasonString(object? value, SerializerOptions options, TasonTypeRegistry registry)
    {
        var sb = new StringBuilder();
        using (var writer = new TasonWriter(sb, options))
        {
            var generator = new TasonGenerator(writer, options, registry);
            generator.Generate(value);
        }
        return sb.ToString();
    }


    public void Generate(object? value)
    {
        Value(value);
        writer.Flush();
    }

    bool Value(object? value, ValueScope scope = ValueScope.Normal)
    {
        if (value == null)
            writer.Write("null");
        else if (value is bool b)
            writer.Write(b ? "true" : "false");
        else if (value is string s)
            writer.WriteString(s);
        else if (TryWriteNumberValue(value, scope))
            return true;
        else if (value is Enum e)
            EnumValue(e);
        else
        {
            var type = value.GetType();
            if (IsBanTypes(type))
            {
                if (scope != ValueScope.ObjectValue)
                    ThrowIfBanTypes(type);
                return false;
            }
            if (TryWriteArrayValue(value))
                return true;
            else 
                MaybeObjectValue(value, type);
        }
        return true;
    }


    void EnumValue(Enum value)
    {
        var baseType = value.GetType().GetEnumUnderlyingType();
        var n = Convert.ChangeType(value, baseType);
        TryWriteNumberValue(n);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBanTypes(Type type)
    {
        return type.IsAbstract
            || type.IsInterface
            || type.IsPointer
#if NET8_0_OR_GREATER
            || type.IsFunctionPointer
#endif
            || typeof(Delegate).IsAssignableFrom(type)
        ;
    }

    private static void ThrowIfBanTypes(Type type)
    {
        if (IsBanTypes(type))
            throw new NotSupportedException($"Cannot serialize abstract, interface, delegete or pointer type {type.Name}.");
    }

}