
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
    readonly TasonSerializerOptions options;
    readonly TasonTypeRegistry registry;
    public TasonGenerator(TasonWriter writer, TasonSerializerOptions options, TasonTypeRegistry registry)
    {
        this.writer = writer;
        this.options = options;
        this.registry = registry;
    }

    /// <summary>
    /// 序列化对象为TASON字符串
    /// </summary>
    /// <param name="value">要序列化的对象</param>
    /// <param name="options"><see cref="TasonSerializerOptions"/></param>
    /// <param name="registry"><see cref="TasonTypeRegistry"/></param>
    /// <returns>TASON字符串</returns>
    public static string GenerateAsString(object? value, TasonSerializerOptions options, TasonTypeRegistry registry)
    {
        var sb = new StringBuilder();
        using (var writer = new TasonStringWriter(sb, options))
        {
            var generator = new TasonGenerator(writer, options, registry);
            generator.Generate(value);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 序列化对象到指定流
    /// </summary>
    /// <param name="stream">输出流</param>
    /// <param name="value">要序列化的对象</param>
    /// <param name="options"><see cref="TasonSerializerOptions"/></param>
    /// <param name="registry"><see cref="TasonTypeRegistry"/></param>
    public static void GenerateToStream(Stream stream, object? value, TasonSerializerOptions options, TasonTypeRegistry registry)
    {
        using (var writer = new TasonStreamWriter(stream, options))
        {
            var generator = new TasonGenerator(writer, options, registry);
            generator.Generate(value);
        }
    }

    /// <summary>
    /// 序列化对象到指定<see cref="TextWriter"/>，该方法使用内存生成字符串并写入
    /// </summary>
    /// <param name="textWriter">输出<see cref="TextWriter"/></param>
    /// <param name="value">要序列化的对象</param>
    /// <param name="options"><see cref="TasonSerializerOptions"/></param>
    /// <param name="registry"><see cref="TasonTypeRegistry"/></param>
    public static void GenerateToWriter(TextWriter textWriter, object? value, TasonSerializerOptions options, TasonTypeRegistry registry)
    {
        textWriter.Write(GenerateAsString(value, options, registry));
        textWriter.Flush();
    }

    /// <summary>
    /// 生成TASON字符串并向<see cref="TasonWriter"/>中写入
    /// </summary>
    /// <param name="value">要序列化的对象</param>
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