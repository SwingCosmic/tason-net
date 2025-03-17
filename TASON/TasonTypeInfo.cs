namespace TASON;

/// <summary>TASON类型实例种类</summary>
public enum TasonTypeInstanceKind
{
    /// <summary>标量类型实例</summary>
    Scalar,
    /// <summary>对象类型实例</summary>
    Object,
}

/// <summary>
/// TASON类型信息公共接口
/// </summary>
public interface ITasonTypeInfo
{
    /// <summary>TASON类型实例种类</summary>
    TasonTypeInstanceKind Kind { get; }
    /// <summary>对应类型的<see cref="System.Type"/></summary>
    Type Type { get; }

}

/// <summary>标量类型实例信息接口，自定义类型可以由此派生</summary>
public interface ITasonScalarType : ITasonTypeInfo
{
    /// <summary>
    /// 将标量类型实例序列化为代表该类型的字符串
    /// </summary>
    /// <param name="value">要序列化的标量类型实例</param>
    /// <param name="options">选项</param>
    /// <returns>代表该类型的字符串</returns>
    string Serialize(object value, SerializerOptions options);
    /// <summary>
    /// 将代表该类型的字符串反序列化为对应标量类型实例
    /// </summary>
    /// <param name="text">代表该类型的字符串</param>
    /// <param name="options">选项</param>
    /// <returns>标量类型实例</returns>
    object Deserialize(string text, SerializerOptions options);
}


public interface ITasonObjectType : ITasonTypeInfo
{
    /// <summary>
    /// 将标量类型实例序列化为代表该类型的字典
    /// </summary>
    /// <param name="value">要序列化的标量类型实例</param>
    /// <param name="options">选项</param>
    /// <returns>代表该类型的字典</returns>
    Dictionary<string, object?> Serialize(object value, SerializerOptions options);
    /// <summary>
    /// 将代表该类型的字典反序列化为对应标量类型实例
    /// </summary>
    /// <param name="dict">代表该类型的字典</param>
    /// <param name="options">选项</param>
    /// <returns>标量类型实例</returns>
    object Deserialize(Dictionary<string, object?> dict, SerializerOptions options);
}