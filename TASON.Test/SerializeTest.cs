namespace TASON.Test;

using System.Text.RegularExpressions;
using TASON;
using TASON.Types.SystemTextJson;
using TASON.Types.NewtonsoftJson;
using System.Text.Json;
using TASON.Types;

public class SerializeTest
{

    JsonSerializerOptions options = null!;

    [SetUp]
    public void Setup()
    {
        options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        // 两种实现可以同时添加互不影响
        TasonSerializer.Default.Registry
            .AddSystemTextJson(options);
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
        s.Registry.CreateObjectType<TreeNode>();
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
    public void DictionaryTest()
    {
        var pairs = "[A({X:1,Y:2}),1],[A({X:2,Y:4}),2]";
        var tason = $"Dictionary({{keyValuePairs:[{pairs}]}})";

        var s = TasonSerializer.Default.Clone();
        s.Options.UseBuiltinDictionary = true;
        s.Registry.CreateObjectType<A>();

        var expect = new ADict()
        {
            [new A { X = 1, Y = 2 }] = 1,
            [new A { X = 2, Y = 4 }] = 2,
        };
        Assert.That(s.Serialize(expect), Is.EqualTo(tason));


        var s2 = TasonSerializer.Default.Clone();
        s2.Options.UseBuiltinDictionary = false;
        s2.Registry.CreateObjectType<A>();

        Assert.Throws<NotSupportedException>(() => s2.Serialize(expect));
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

    [Test]
    public void ExtensionFields()
    {
        var s = TasonSerializer.Default.Clone();
        s.Registry.CreateObjectType<DynamicFieldClass>();

        var tason = "{NormalProperty:\"foo\",a:1,b:2}";
        var typeTason = $"DynamicFieldClass({tason})";
        var obj = new DynamicFieldClass()
        {
            NormalProperty = "foo",
            DynamicFields = new Dictionary<string, object?>()
            {
                ["a"] = 1,
                ["b"] = 2,
            }
        };

        Assert.That(s.Serialize(obj), Is.EqualTo(typeTason));


        var s2 = TasonSerializer.Default.Clone();
        Assert.That(s2.Serialize(obj), Is.EqualTo(tason));
    }
}
