namespace TASON.Test;

using System.Text.RegularExpressions;
using TASON;
using TASON.Types.SystemTextJson;
using System.Text.Json;

public class SerializeTest
{

    record class A
    {
        public int X { get; set; }
        public int Y { get; set; }
    }


    class ADict : Dictionary<A, int>
    {

    }


    JsonSerializerOptions options = null!;

    [SetUp]
    public void Setup()
    {
        options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        TasonSerializer.Default.Registry
            .AddSystemTextJson(options);
    }


    [Test]
    public void Primitives()
    {
        var s = new TasonSerializer(new SerializerOptions
        {
            Indent = 2,
        }, TasonSerializer.Default.Registry.Clone());
        long? int64 = 0xabcdL;
        var array = new object[] 
        { 
            1, 
            new Dictionary<string, object> 
            {
                ["a"] = 1,
            },
            int64
        };
        Assert.That(s.Serialize(array), Is.EqualTo(
new Regex("[\r\n]+").Replace("""
[
  1,
  {
    a: 1
  },
  Int64("43981")
]
""", Environment.NewLine)));
    }
    [Test]
    public void JSON()
    {
        var s = TasonSerializer.Default;

        var json = new JSON("[1,2,16777215]", options, JSONSubType.Array);
        Assert.That(s.Serialize(json), Is.EqualTo("JSONArray(\"[1,2,16777215]\")"));
        
        var json2 = new JSON(@"{""啊？\r\n"":1}", options, JSONSubType.Object);
        Assert.That(s.Serialize(json2), Is.EqualTo(@"JSONObject(""{\""啊？\\r\\n\"":1}"")"));
    }

    [Test]
    public void RegExpTest()
    {
        var s = TasonSerializer.Default;

        var reg = new Regex("[a-z]+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Assert.That(s.Serialize(reg), Is.EqualTo("RegExp(\"/[a-z]+/imu\")"));
    }

    [Test]
    public void DictionaryTest()
    {
        var pairs = "[A({X:1,Y:2}),1],[A({X:2,Y:4}),2]";
        var tason = $"Dictionary({{keyValuePairs:[{pairs}]}})";

        var s = TasonSerializer.Default.Clone();
        s.Options.UseBuiltinDictionary = true;
        s.Registry.CreateObjectType(typeof(A));

        var expect = new ADict()
        {
            [new A { X = 1, Y = 2 }] = 1,
            [new A { X = 2, Y = 4 }] = 2,
        };
        Assert.That(s.Serialize(expect), Is.EqualTo(tason));


        var s2 = TasonSerializer.Default.Clone();
        s2.Options.UseBuiltinDictionary = false;
        s2.Registry.CreateObjectType(typeof(A));

        Assert.Throws<NotSupportedException>(() => s2.Serialize(expect));
    }
}
