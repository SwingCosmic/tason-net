
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TASON;

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

    string? Value(object? value)
    {
        if (value == null)
            return "null";
        else if (value is bool b)
            return BooleanValue(b);
        else if (value is string s)
            return StringValue(s);
        else if (TryGetNumberValue(value, out var number)) 
            return number;
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string BooleanValue(bool value)
    {
        return value ? "true" : "false";
    }


    string TypeInstanceValue(object value, ITasonTypeInfo type) 
    {
        return "";
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
}