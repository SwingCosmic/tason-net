namespace TASON.Test;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TASON;
using TASON.Types;
using TASON.Types.SystemTextJson;
using System.Text.Json;

public class ParseTest
{
    [SetUp]
    public void Setup()
    {
        TasonSerializer.Default.Registry
            .AddSystemTextJson(new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    [Test]
    public void Primitives()
    {
        var s = TasonSerializer.Default;
        Assert.That(s.Deserialize("null"), Is.EqualTo(null));
        Assert.That(s.Deserialize("[0o777, 'ds', [Infinity, [Int64('-0xabCDef123456789')]]]"),
            Is.EqualTo(new object[] 
            { 
                511, 
                "ds", 
                new object[] 
                { 
                    double.PositiveInfinity,
                    new object[]
                    {
                        -0xabCDef123456789L
                    }
                } 
            }));

        Assert.That(s.Deserialize("{\"a\": true, b: \"fo\\n\\ro\"}"),
            Is.EqualTo(new Dictionary<string, object?> 
            { 
                ["a"] = true, 
                ["b"] = "fo\n\ro" 
            }));
    }

    [Test]
    public void Types()
    {
        var s = TasonSerializer.Default;

        var reg = s.Deserialize("RegExp('/([A-Z]+)\\\\1/mi')") as Regex;
        var expect = new Regex("([A-Z]+)\\1", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Assert.That(RegexComparer.ToString(reg!), Is.EqualTo(RegexComparer.ToString(expect)));

        var json = s.Deserialize("JSONArray('[1,2,3]')") as JSON;
        Assert.That(json!.GetValue<List<int>>(), Is.EqualTo(new List<int> { 1, 2, 3 }));
    }
}

public class RegexComparer : EqualityComparer<Regex>
{
    static RegExpType RegExp = new();
    static SerializerOptions options = new();
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