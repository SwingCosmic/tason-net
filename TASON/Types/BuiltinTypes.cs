
using System.Collections.ObjectModel;

namespace TASON.Types;

public static class BuiltinTypes
{
    /// <summary>内置类型列表</summary>
    public static ReadOnlyDictionary<string, ITasonTypeInfo> Types { get; } = new(
        new Dictionary<string, ITasonTypeInfo>
        ([
            ..NumberTypes.Types,
            new("UUID", new UUIDType()),
            new("RegExp", new RegExpType()),
            new("Buffer", new BufferType()),
        ])
    );
    
    /// <summary>内置类型列表</summary>
    public static ReadOnlyDictionary<string, string> Aliases { get; } = new(
        new Dictionary<string, string>
        ([
            ..NumberTypes.Aliases,
        ])
    );

    /// <summary>鸭子类型实现列表，即多种.NET类型序列化为同一种TASON类型</summary>
    public static ReadOnlyDictionary<string, ITasonTypeInfo[]> DuckTypes { get; } = new(
        new Dictionary<string, ITasonTypeInfo[]>
        ([
            new("UInt16", [NumberTypes.Char]), // char和ushort在内存的表示完全一致
        ])
    );

}
