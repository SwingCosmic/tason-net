namespace TASON;

/// <summary>
/// 找不到指定TASON类型时引发的异常，通常是由于未注册或者名称拼写错误
/// </summary>
[Serializable]
public class TasonTypeNotFoundException : Exception
{
    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; }
    static string FormatMessgae(string typeName) => $"TASON type '{typeName}' does not exist";
    /// <inheritdoc/>
    public TasonTypeNotFoundException() : base($"TASON type does not exist") 
    {
        TypeName = "";
    }
    /// <inheritdoc/>
    public TasonTypeNotFoundException(string typeName) : base(FormatMessgae(typeName))
    {
        TypeName = typeName;
    }
    /// <inheritdoc/>
    public TasonTypeNotFoundException(string typeName, Exception inner) : base(FormatMessgae(typeName), inner)
    {
        TypeName = typeName;
    }
    /// <inheritdoc/>
    protected TasonTypeNotFoundException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}