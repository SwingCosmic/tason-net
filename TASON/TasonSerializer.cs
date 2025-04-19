using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using TASON.Grammar;

namespace TASON;

/// <summary>
/// TASON序列化器
/// </summary>
public class TasonSerializer
{
    /// <summary>
    /// 序列化选项
    /// </summary>
    public TasonSerializerOptions Options { get; }
    /// <summary>
    /// 类型注册表
    /// </summary>
    public TasonTypeRegistry Registry { get; }

    /// <summary>
    /// 默认TASON序列化器
    /// </summary>
    public static TasonSerializer Default { get; } = new TasonSerializer();

    /// <summary>
    /// 创建<see cref="TasonSerializer"/>的新实例
    /// </summary>
    /// <param name="options">序列化选项</param>
    /// <param name="registry">类型注册表</param>
    public TasonSerializer(TasonSerializerOptions? options = null, TasonTypeRegistry? registry = null)
    {
        options ??= new TasonSerializerOptions();
        Options = options;

        registry ??= new TasonTypeRegistry(options);
        Registry = registry;
    }
    /// <summary>
    /// 将TASON字符串反序列化为.NET对象，类型根据TASON描述信息自动推断
    /// </summary>
    /// <param name="text">TASON字符串</param>
    /// <returns>反序列化的.NET对象</returns>
    public object? Deserialize(string text)
    {
        var lexer = new TASONLexer(new AntlrInputStream(text));
        var parser = new TASONParser(new CommonTokenStream(lexer));

        var visitor = new TasonVisitor(Registry, Options);
        return visitor.Start(parser.start());
    }

    /// <summary>
    /// 将TASON字符串反序列化为指定类型的.NET对象
    /// </summary>
    /// <param name="text">TASON字符串</param>
    /// <param name="type">类型</param>
    /// <returns>反序列化的.NET对象</returns>
    public object? Deserialize(string text, Type type)
    {
        var lexer = new TASONLexer(new AntlrInputStream(text));
        var parser = new TASONParser(new CommonTokenStream(lexer));
        parser.AddErrorListener(new ThrowingErrorListener());

        var visitor = new TasonVisitor(Registry, Options);
        return visitor.StartDeserialize(parser.start(), type);
    }
    
    /// <summary>
    /// 将TASON字符串反序列化为指定类型的.NET对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="text">TASON字符串</param>
    /// <returns>反序列化的.NET对象</returns>
    public T? Deserialize<T>(string text) where T : notnull
    {
        var lexer = new TASONLexer(new AntlrInputStream(text));
        var parser = new TASONParser(new CommonTokenStream(lexer));
        parser.AddErrorListener(new ThrowingErrorListener());

        var visitor = new TasonVisitor(Registry, Options);
        return visitor.StartDeserialize<T>(parser.start());
    }

    /// <summary>
    /// 将代表TypeInstance的TASON字符串反序列化为.NET对象，类型为<typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">TypeInstance对应的CLR类型</typeparam>
    /// <param name="text">TASON字符串</param>
    /// <returns>反序列化的.NET对象</returns>
    public T DeserializeTypeInstance<T>(string text) where T : notnull
    {
        var lexer = new TASONLexer(new AntlrInputStream(text));
        var parser = new TASONParser(new CommonTokenStream(lexer));

        var visitor = new TasonVisitor(Registry, Options);
        return visitor.StartTypeInstanceValue<T>(parser.start());
    }

    /// <summary>
    /// 将.NET对象序列化为TASON字符串
    /// </summary>
    /// <param name="value">待序列化的对象</param>
    /// <returns>TASON字符串</returns>
    public string Serialize(object? value)
    {
        return TasonGenerator.GenerateAsString(value, Options, Registry);
    }

    /// <summary>
    /// 将.NET对象序列化为TASON字符串并写入<see cref="TextWriter"/>
    /// </summary>
    /// <param name="value">待序列化的对象</param>
    /// <param name="writer">要写入的<see cref="TextWriter"/></param>
    public void Serialize(object? value, TextWriter writer)
    {
        TasonGenerator.GenerateToWriter(writer, value, Options, Registry);
    }


    /// <summary>
    /// 根据当前的<see cref="Options"/>和<see cref="Registry"/>创建<see cref="TasonSerializer"/>的副本
    /// </summary>
    /// <returns>新的副本</returns>
    public TasonSerializer Clone()
    {
        var options = Options with { };
        var registry = Registry.Clone(options);
        return new TasonSerializer(options, registry);
    }
}
