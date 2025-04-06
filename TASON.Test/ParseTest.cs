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

    record class A
    {
        public int X { get; set; }
        public int Y { get; set; }
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

        s.Registry.CreateObjectType(typeof(A));
        Assert.That(s.Deserialize("A({\nX:Int('1'),Y:Int('2')\n})"), Is.EqualTo(new A() { X = 1, Y = 2 }));
    }

    [Test]
    public void Date()
    {
        var s = TasonSerializer.Default;

        var date = (DateTime)s.Deserialize("Date('2020-01-15T09:00:00.666Z')")!;
        Assert.That(date, Is.EqualTo(new DateTime(2020, 1, 15, 9, 0, 0, 666, DateTimeKind.Utc)));
        
        var date2 = (DateTime)s.Deserialize("Date('2024-02-10 00:00:00-07:00')")!;
        date2 = date2.ToUniversalTime();
        Assert.That(date2, Is.EqualTo(new DateTimeOffset(2024, 2, 10, 0, 0, 0, TimeSpan.FromHours(-7)).UtcDateTime));

        var date3 = (DateTime)s.Deserialize("Date('2025-03-31 09:40:00')")!;
        Assert.That(date3, Is.EqualTo(new DateTime(2025, 3, 31, 9, 40, 0, DateTimeKind.Local)));

        var offset = DateTimeOffset.UtcNow;
        var timestamp = (Timestamp)s.Deserialize($"Timestamp('{offset.Millisecond + 1000}')")!;
        Assert.That(timestamp, Is.EqualTo(new Timestamp(offset.Millisecond) + 1000));
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