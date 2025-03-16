namespace TASON.Test;
using TASON;
public class ParseTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Primitives()
    {
        var s = TasonSerializer.Default;
        Assert.That(s.Deserialize("null"), Is.EqualTo(null));
        Assert.That(s.Deserialize("[0o777, 'ds', [Infinity, []]]"),
            Is.EqualTo(new object[] 
            { 
                511, 
                "ds", 
                new object[] 
                { 
                    double.PositiveInfinity,
                    Array.Empty<object>()
                } 
            }));

        Assert.That(s.Deserialize("{\"a\": true, 'b': \"fo\\n\\ro\"}"),
            Is.EqualTo(new Dictionary<string, object?> 
            { 
                ["a"] = true, 
                ["b"] = "fo\n\ro" 
            }));
    }
}