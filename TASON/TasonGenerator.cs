
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TASON;

internal enum ValueScope 
{
    Normal,
    ObjectValue,
}

public partial class TasonGenerator
{
    readonly SerializerOptions options;
    readonly TasonTypeRegistry registry;
    public TasonGenerator(SerializerOptions options, TasonTypeRegistry registry)
    {
        this.options = options;
        this.registry = registry;
    }

    private int indentLevel = 0;

    public string Generate(object? value)
    {
        return Value(value)!;
    }
    string? Value(object? value, ValueScope scope = ValueScope.Normal)
    {
        if (value == null)
            return "null";
        else if (value is bool b)
            return BooleanValue(b);
        else if (value is string s)
            return StringValue(s);
        else if (TryGetNumberValue(value, out var number)) 
            return number;
        else if (value is Enum e)
            return EnumValue(e);
        else
        {
            var type = value.GetType();
            if (IsBanTypes(type))
            {
                if (scope != ValueScope.ObjectValue)
                    ThrowIfBanTypes(type);
                return null;
            }
            if (TryGetArrayValue(value, out var array))
                return array;
            else 
                return MaybeObjectValue(value, type);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string BooleanValue(bool value)
    {
        return value ? "true" : "false";
    }

    string EnumValue(Enum value)
    {
        var baseType = value.GetType().GetEnumUnderlyingType();
        var n = Convert.ChangeType(value, baseType);
        TryGetNumberValue(n, out var ret);
        return ret!;
    }


    private static readonly JsonSerializerOptions jsonStringOption = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string StringValue(string value)
    {
        return JsonSerializer.Serialize(value, jsonStringOption);
    }


    private void CheckDepth()
    {
        if (indentLevel > options.MaxDepth)
        {
            throw new StackOverflowException(
              "Maximum object or array depth exceeded. Is there a circular reference?"
            );
        }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string Indent() => options.Indent is null 
        ? string.Empty
        : new string(' ', options.Indent.Value * indentLevel);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string Space() => options.Indent is null 
        ? string.Empty
        : " ";
}