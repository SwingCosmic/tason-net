using System.Reflection;
using TASON;

namespace Microsoft.AspNetCore.Mvc;

public class MvcTasonOptions
{
    public SerializerOptions SerializerOptions { get; set; } = TasonSerializer.Default.Options with { };

    public TasonTypeRegistry TypeRegistry { get; set; } = TasonSerializer.Default.Registry.Clone();

    /// <summary>
    /// 指定需要自动注册为<see cref="ITasonObjectType"/>的类型，这些类型要求提供无参构造函数，通过反射进行属性查找。
    /// </summary>
    public Func<Type[]>? GetAutoRegisterObjectTypes { get; set; }

}
