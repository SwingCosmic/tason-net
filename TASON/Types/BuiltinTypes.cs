
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
        ])
    );
    
    /// <summary>内置类型列表</summary>
    public static ReadOnlyDictionary<string, string> Aliases { get; } = new(
        new Dictionary<string, string>
        ([
            ..NumberTypes.Aliases,
        ])
    );

}
