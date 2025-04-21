namespace TASON.Test;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TASON;
using TASON.Types;

public class RegexComparer : EqualityComparer<Regex>
{
    static RegExpType RegExp = new();
    static TasonSerializerOptions options = new();
    static Dictionary<Regex, bool> testMap = new(new RegexComparer());
    public override bool Equals(Regex? x, Regex? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return ToString(x) == ToString(y);
    }

    public static string ToString(Regex obj)
    {
        return RegExp.Serialize(obj, options);
    }

    public override int GetHashCode([DisallowNull] Regex obj)
    {
        return ToString(obj).GetHashCode();
    }

    public static bool IsEqual(Regex obj1, Regex obj2)
    {
        testMap[obj1] = true;
        var ret = testMap.ContainsKey(obj2);
        testMap.Clear();
        return ret;
    }

}