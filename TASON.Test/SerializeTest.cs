namespace TASON.Test;

using System.Text.RegularExpressions;
using TASON;
using TASON.Types.SystemTextJson;
using TASON.Types.NewtonsoftJson;
using System.Text.Json;
using TASON.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

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

    record class TreeNode
    {
        public string Name { get; set; }
        public TreeNode[] Children { get; set; } = [];
    }


    JsonSerializerOptions options = null!;
    JsonSerializerSettings setting = null!;

    [SetUp]
    public void Setup()
    {
        options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        setting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        // 两种实现可以同时添加互不影响
        TasonSerializer.Default.Registry
            .AddSystemTextJson(options)
            .AddNewtonsoftJson(setting);
    }


    [Test]
    public void Option_Indent()
    {
        var s = new TasonSerializer(new TasonSerializerOptions
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
    public void CloneTest()
    {
        var s = TasonSerializer.Default.Clone();
        s.Options.Indent = 10;

        Assert.Multiple(() =>
        {
            Assert.That(TasonSerializer.Default.Options.Indent, Is.EqualTo(null));
            Assert.That(s.Registry.Options.Indent, Is.EqualTo(10));
        });

    }


    [Test]
    public void Option_MaxDepth()
    {
        var s = TasonSerializer.Default.Clone();
        s.Registry.CreateObjectType(typeof(TreeNode));
        var node = new TreeNode
        {
            Name = "node0",
            Children = [],
        };
        for (int i = 1; i < 100; i++)
        {
            node = new TreeNode
            {
                Name = "node" + i,
                Children = [node],
            };
        }


        Assert.Throws<StackOverflowException>(() => s.Serialize(node));
        // 实际应该大于循环次数99的2倍，因为TreeNode类每一个实例就有一层{}和一层[]；
        // 由于Writer实现的差异会比JavaScript版本多一层
        s.Options.MaxDepth = 200;
        Assert.DoesNotThrow(() => s.Serialize(node));
    }

    [Test]
    public void JSON()
    {
        var s = TasonSerializer.Default;

        var sjson = new JSONSystemText("[1,2,16777215]", options, JSONSubType.Array);
        var njson = new JSONNewtonsoft("[1,2,16777215]", setting, JSONSubType.Array);
        Assert.Multiple(() =>
        {
            Assert.That(s.Serialize(sjson), Is.EqualTo("JSONArray(\"[1,2,16777215]\")"));
            Assert.That(s.Serialize(njson), Is.EqualTo("JSONArray(\"[1,2,16777215]\")"));
        });

        var sjson2 = new JSONSystemText(@"{""啊？\r\n"":1}", options, JSONSubType.Object);
        var njson2 = new JSONNewtonsoft(@"{""啊？\r\n"":1}", setting, JSONSubType.Object);
        Assert.Multiple(() =>
        {
            Assert.That(s.Serialize(sjson2), Is.EqualTo(@"JSONObject(""{\""啊？\\r\\n\"":1}"")"));
            Assert.That(s.Serialize(njson2), Is.EqualTo(@"JSONObject(""{\""啊？\\r\\n\"":1}"")"));
        });
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


    class IndexerClass
    {
        Dictionary<string, string?> dict = new();
        public string? this[string key]
        {
            get => dict.GetValueOrDefault(key, null);
            set => dict[key] = value;
        }

        public string NormalProperty { get; set; } = "";
    }

    [Test]
    public void IndexerTest()
    {
        var s = TasonSerializer.Default;

        var obj = new IndexerClass
        {
            NormalProperty = "a",
        };
        obj["indexer"] = "b";
        Assert.That(s.Serialize(obj), Is.EqualTo("""{NormalProperty:"a"}"""));
    }
}
