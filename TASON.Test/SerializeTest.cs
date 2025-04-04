namespace TASON.Test;

using System.Text.RegularExpressions;
using TASON;
using TASON.Types.SystemTextJson;
using System.Text.Json;

public class SerializeTest
{
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

        var array = new object[] 
        { 
            1, 
            new Dictionary<string, object> 
            {
                ["a"] = 1,
            }, 
            0xabcdL 
        };
        Assert.That(s.Serialize(array), Is.EqualTo(
"""
[
  1,
  {
    a: 1
  },
  Int64("43981")
]
""".ReplaceLineEndings("\n")
            ));
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
}
