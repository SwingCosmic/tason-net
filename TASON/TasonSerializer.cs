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
    public SerializerOptions Options { get; }
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
    public TasonSerializer(SerializerOptions? options = null, TasonTypeRegistry? registry = null)
    {
        options ??= new SerializerOptions();
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

    public T DeserializeObjectType<T>(string text) where T : class
    {
        var obj = Deserialize(text)!;
        return (T)obj;
    }

    /// <summary>
    /// 将.NET对象序列化为TASON字符串
    /// </summary>
    /// <param name="value">待序列化的对象</param>
    /// <returns>TASON字符串</returns>
    public string Serialize(object? value)
    {
        var generator = new TasonGenerator(Options, Registry);
        return generator.Generate(value);
    }
}
