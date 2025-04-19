using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using TASON.Grammar;
using TASON.Util;

namespace TASON;

/// <summary>
/// 提供给<see cref="TasonGenerator"/>以实现顺序写入<see cref="TextWriter"/>，从而避免字符串拼接的性能问题
/// </summary>
public class TasonWriter : IDisposable
{
    
    readonly TextWriter writer;
    readonly TasonSerializerOptions options;

    const char StartObject = '{';
    const char EndObject = '}';
    const char StartArray = '[';
    const char EndArray = ']';
    const char DoubleQuote = '"';
    const char SingleQuote = '\'';
    const char OneSpace = ' ';
    readonly string NewLine = Environment.NewLine;
    readonly string Separator;
    readonly string Space;

    private int indentLevel = 0;

    /// <summary>
    /// 直接使用<see cref="TextWriter"/>创建<see cref="TasonWriter"/>的新实例
    /// </summary>
    /// <param name="writer">用于写入字符串的<see cref="TextWriter"/></param>
    /// <param name="options">序列化选项</param>
    public TasonWriter(TextWriter writer, TasonSerializerOptions? options = null) 
    {
        options ??= new();
        this.options = options;
        if (options.Indent < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Indent cannot be negative");
        }

        Separator = options.Indent is null ? "," : $",{NewLine}";
        Space = options.Indent is null ? string.Empty : " ";

        this.writer = writer;
    }

    /// <summary>
    /// 使用<see cref="StringBuilder"/>创建<see cref="TasonWriter"/>的新实例
    /// </summary>
    /// <param name="sb">用于写入字符串的<see cref="StringBuilder"/></param>
    /// <param name="options">序列化选项</param>
    public TasonWriter(StringBuilder sb, TasonSerializerOptions? options = null) 
        : this(new StringWriter(sb), options)
    {
    }

    /// <summary>
    /// 使用<see cref="Stream"/>创建<see cref="TasonWriter"/>的新实例
    /// </summary>
    /// <param name="stream">用于写入字符串的流</param>
    /// <param name="options">序列化选项</param>
    /// <exception cref="IOException"><paramref name="stream"/>不是可写的</exception>
    public TasonWriter(Stream stream, TasonSerializerOptions? options = null)
        : this(CreateWriterFromStream(stream), options)
    {
    }

    static TextWriter CreateWriterFromStream(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new IOException("Stream is not writable");
        }
        return new StreamWriter(stream);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        writer.Dispose();
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string Indent() => options.Indent is null 
        ? string.Empty
        : new string(OneSpace, options.Indent.Value * indentLevel);

    void CheckDepth()
    {
        if (indentLevel > options.MaxDepth)
        {
            throw new StackOverflowException(
              "Maximum object or array depth exceeded. Is there a circular reference?"
            );
        }
    }

    /// <summary>
    /// 写入一个原始的TASON字符串
    /// </summary>
    /// <param name="value">TASON字符串</param>
    public void Write(string value)
    {
        writer.Write(value);
    }    

    /// <summary>
    /// 刷新缓冲区，通常用于结束写入
    /// </summary>
    public void Flush()
    {
        writer.Flush();
    }

    /// <summary>
    /// 写入一个带有 '"' 且经过转义的字符串
    /// </summary>
    /// <param name="value">字符串</param>
    public void WriteString(string value)
    {
        writer.Write($"{DoubleQuote}{value.Escape()}{DoubleQuote}");
    }

    /// <summary>
    /// 写入对象开始的 '{' 并提升缩进级别
    /// </summary>
    public void WriteStartObject()
    {
        writer.Write(StartObject);
        if (options.Indent is not null)
        {
            writer.Write(NewLine);
        }  
        
        indentLevel++;
        CheckDepth();
    }

    /// <summary>
    /// 写入对象结束的 '}' 并降低缩进级别
    /// </summary>
    public void WriteEndObject()
    {
        indentLevel--;

        if (options.Indent is not null)
        {
            writer.Write(NewLine);
            writer.Write(Indent());
        }
        writer.Write(EndObject);
    }

    /// <summary>
    /// 写入一个空的对象
    /// </summary>
    public void WriteEmptyObject()
    {
        writer.Write("{}");
    }

    /// <summary>
    /// 写入数组开始的 '[' 并提升缩进级别
    /// </summary>
    public void WriteStartArray()
    {
        writer.Write(StartArray);
        if (options.Indent is not null)
        {
            writer.Write(NewLine);
        }

        indentLevel++;
        CheckDepth();
    }

    /// <summary>
    /// 写入数组结束的 ']' 并降低缩进级别
    /// </summary>
    public void WriteEndArray()
    {
        indentLevel--;

        if (options.Indent is not null)
        {
            writer.Write(NewLine);
            writer.Write(Indent());
        }
        writer.Write(EndArray);
    }

    /// <summary>
    /// 写入一个空的数组
    /// </summary>
    public void WriteEmptyArray()
    {
        writer.Write("[]");
    }


    /// <summary>
    /// 遍历一个数组，将数组中的元素通过 <paramref name="tryWrite"/> 写入并使用 <see cref="Separator"/> 分隔
    /// </summary>
    /// <param name="tryWrite">尝试写入一个数组元素的方法，
    /// 返回 <see langword="true"/> 表示写入成功，<see langword="false"/> 表示跳过该元素</param>
    /// <param name="values">要遍历的数组</param>
    public void WriteJoin<T>(Func<T, bool> tryWrite, IList<T> values) 
    {
        var len = values.Count;
        if (len == 0) return; 
        for (var i = 0; i < len; i++)
        {
            if (!tryWrite(values[i])) continue;
            if (i < len - 1)
            {
                writer.Write(Separator);
            }
        }
    }

    /// <summary>
    /// 写入一个对象的键值对
    /// </summary>
    /// <param name="key">写入值的方法</param>
    /// <param name="value">写入值的方法</param>
    public void WriteObjectPair(Action key, Action value)
    {
        writer.Write(Indent());
        key();
        writer.Write($":{Space}");
        value();
    }

    /// <summary>
    /// 写入一个数组的元素
    /// </summary>
    /// <param name="value">写入值的方法</param>
    public void WriteArrayItem(Action value) 
    {
        writer.Write(Indent());
        value();
    }

    /// <summary>
    /// 写入一个类型实例
    /// </summary>
    /// <param name="name">类型名</param>
    /// <param name="arg">写入参数的方法</param>
    public void WriteTypeInstance(string name, Action arg)
    {
        writer.Write($"{name}(");
        arg();
        writer.Write(")");
    }
}
