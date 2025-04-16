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

/// <summary>对象类型实例信息接口，自定义类型可以由此派生</summary>
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


/// <summary>
/// 表示带有类型名称的<see cref="ITasonTypeInfo"/>。
/// 采用<see langword="readonly"/> <see langword="struct"/>是因为此类型仅作为临时状态返回，不应该修改和长期持有。
/// </summary>
public readonly record struct TasonNamedTypeInfo
{
    /// <summary>TASON类型的名称</summary>
    public string Name { get; }
    /// <summary>封装的<see cref="ITasonTypeInfo"/></summary>
    public ITasonTypeInfo TypeInfo { get; }
    /// <summary>
    /// 创建<see langword="readonly"/> <see langword="struct"/> <see cref="TasonNamedTypeInfo"/>的新实例
    /// </summary>
    /// <param name="name">TASON类型的名称</param>
    /// <param name="typeInfo">该TASON类型对应的<see cref="ITasonTypeInfo"/></param>
    public TasonNamedTypeInfo(string name, ITasonTypeInfo typeInfo)
    {
        Name = name;
        TypeInfo = typeInfo;
    }

}