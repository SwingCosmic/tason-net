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


    }

    [Test]
    public void ObjectTest()
    {
        var s = TasonSerializer.Default;

        Assert.That(s.Deserialize("{\"a\": true, b: \"fo\\n\\ro\"}"),
            Is.EqualTo(new Dictionary<string, object?>
            {
                ["a"] = true,
                ["b"] = "fo\n\ro"
            }));

        var s2 = s.Clone();
        s2.Registry.CreateObjectType<A>();
        Assert.That(s2.Deserialize("A({X:1,Y:2})"), Is.EqualTo(new A() { X = 1, Y = 2 }));
    }



}
