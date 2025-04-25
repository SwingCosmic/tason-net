namespace TASON.Test;

using System.Text.RegularExpressions;
using TASON;
using TASON.Types.SystemTextJson;
using TASON.Types.NewtonsoftJson;
using System.Text.Json;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TASON.Util;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

public class ParseGenericTest
{
    

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
    public void EnumTest()
    {
        var s = TasonSerializer.Default;
        var enumValue = s.Deserialize<StringSplitOptions>("1");
        Assert.That(enumValue, Is.EqualTo(StringSplitOptions.RemoveEmptyEntries));

        var flagValue = s.Deserialize<BindingFlags[]>("[0b01000, 0b10000, 0b11000]");
        Assert.That(flagValue, Is.EqualTo(new[] 
        { 
            BindingFlags.Static, 
            BindingFlags.Public, 
            BindingFlags.Static | BindingFlags.Public 
        }));
    }


    [Test]
    public void CollectionTest()
    {
        var s = TasonSerializer.Default;
        var array = s.Deserialize<long[]>("[Long('1'),0xeeeeeeeeeeee,Int64('-114514')]");
        Assert.That(array, Is.EqualTo(new long[] { 1, 0xeeeeeeeeeeeeL, -114514 }));

        var stringListTason = "['a', 'b', '\\n']";
        var stringList = new[] { "a", "b", "\n" };


        var list = s.Deserialize<ISet<string>>(stringListTason);
        Assert.That(list, Is.EqualTo(new HashSet<string>(stringList)));
        
        var list2 = s.Deserialize<TestList>(stringListTason);
        Assert.That(list2, Is.EqualTo(new TestList(stringList)));
        
        var list3 = s.Deserialize<ReadOnlyCollection<string>>(stringListTason);
        Assert.That(list3, Is.EqualTo(new ReadOnlyCollection<string>(stringList)));
    }

    [Test]
    public void ObjectTest()
    {
        var s = TasonSerializer.Default.Clone();

        s.Registry.CreateObjectType<A>();
        Assert.That(s.Deserialize<A>("{X:1, Y:2, }"), Is.EqualTo(new A() { X = 1, Y = 2 }));


    }    
    
    
    [Test]
    public void DictionaryTest()
    {
        var pairs = "[A({X:1,Y:2}),1],[A({X:2,Y:4}),2]";
        var tason = $"Dictionary({{keyValuePairs:[{pairs}]}})";

        // 测试UseBuiltinDictionary=true的非string key字典
        var s = TasonSerializer.Default.Clone();
        s.Options.UseBuiltinDictionary = true;
        s.Registry.CreateObjectType<A>();

        Assert.That(s.Deserialize<ADict>(tason), 
            Is.EqualTo(new ADict() 
            {
                [new A { X = 1,Y = 2 }] = 1,
                [new A { X = 2,Y = 4 }] = 2,
            }));

        // 测试UseBuiltinDictionary=false的非string key字典，应该报错
        var s2 = TasonSerializer.Default.Clone();
        s2.Options.UseBuiltinDictionary = false;
        s2.Registry.CreateObjectType<A>();

        Assert.Throws<ArgumentException>(() => s2.Deserialize<ADict>(tason));
    }

    [Test]
    public void JsonTest()
    {
        var s = TasonSerializer.Default;

        var json = """
{
    "tags": {
        "a": "foo",
        "b": "bar"
    }
}
""";
        var expects = ReadSJson(json);
        var expectn = ReadNJson(json);

        // 测试JsonDocument
        var tason = $"JSON(\"{PrimitiveHelpers.Escape(json)}\")";
        Assert.Multiple(() =>
        {
            var objs = s.Deserialize<JsonDocument>(tason)!;
            var objn = s.Deserialize<JToken>(tason)!;

            Assert.That(objs.RootElement.ToString(), Is.EqualTo(expects.RootElement.ToString()));
            Assert.That(objn.ToString(), Is.EqualTo(expectn.ToString()));
        });

        // 测试含有JsonElement的嵌套字典
        var inner = """{"a":"foo","b":"bar"}""";
        var tason2 = "{'tags': " + $"JSON(\"{PrimitiveHelpers.Escape(inner)}\")" + "}";

        Assert.Multiple(() =>
        {
            var objs2 = s.Deserialize<Dictionary<string, JsonElement>>(tason2)!;
            var objn2 = s.Deserialize<Dictionary<string, JToken>>(tason2)!;

            Assert.That(System.Text.Json.JsonSerializer.Serialize(objs2, options),
                Is.EqualTo(System.Text.Json.JsonSerializer.Serialize(expects.RootElement, options)));
            Assert.That(JsonConvert.SerializeObject(objn2, setting),
                Is.EqualTo(JsonConvert.SerializeObject(expectn, setting)));
        });

    }

    JsonDocument ReadSJson([StringSyntax(StringSyntaxAttribute.Json)] string json)
    {
        return JsonDocument.Parse(json, options.GetDocumentOptions());
    }
    
    JToken ReadNJson([StringSyntax(StringSyntaxAttribute.Json)] string json)
    {
        return JToken.Parse(json);
    }


    [Test]
    public void NullableTest()
    {
        var s = TasonSerializer.Default;

        var tason = "{a:1,b: null,c: -88}";
        Assert.That(s.Deserialize<Dictionary<string, int?>>(tason), Is.EqualTo(new Dictionary<string, int?>()
        {
            ["a"] = 1,
            ["b"] = null,
            ["c"] = -88,
        }));

        var tason2 = "{IntVal:null,DateTimeVal:Date('2025-04-18 00:00:00Z')}";
        Assert.That(s.Deserialize<NullableClass>(tason2), Is.EqualTo(new NullableClass()
        {
            IntVal = null,
            DateTimeVal = new DateTime(2025,4,18,0,0,0,DateTimeKind.Utc),
        }));
    }


    [Test]
    public void AnonymousClass()
    {
        var anonymousClass = new { a = 1, b = "foo" };

        var tason = "{a:1,b:'foo'}";
        Assert.Throws<InvalidOperationException>(() => 
            TasonSerializer.Default.Deserialize(tason, anonymousClass.GetType()));

    }


    [Test]
    public void ExtensionFields()
    {
        var s = TasonSerializer.Default.Clone();
        // 注册类型，走类型实例
        s.Registry.CreateObjectType<DynamicFieldClass>();

        var tason = "DynamicFieldClass({NormalProperty:\"foo\",a:1,b:2})";
        var expect = new DynamicFieldClass()
        {
            NormalProperty = "foo",
            DynamicFields = new Dictionary<string, object?>()
            {
                ["a"] = 1,
                ["b"] = 2,
            }
        };

        var actual = s.Deserialize<DynamicFieldClass>(tason)!;
        Assert.Multiple(() =>
        {
            Assert.That(actual.NormalProperty, Is.EqualTo(expect.NormalProperty));
            Assert.That(actual.DynamicFields, Is.EqualTo(expect.DynamicFields));
        });


        var s2 = TasonSerializer.Default.Clone();
        // 不注册类型，走对象

        var tason2 = "{NormalProperty:\"foo\",a:1,b:2}";
        var actual2 = s2.Deserialize<DynamicFieldClass>(tason2)!;
        Assert.Multiple(() =>
        {
            Assert.That(actual2.NormalProperty, Is.EqualTo(expect.NormalProperty));
            Assert.That(actual2.DynamicFields, Is.EqualTo(expect.DynamicFields));
        });
    }


    [Test]
    public void PolymorphismInstance()
    {
        var s = TasonSerializer.Default.Clone();
        s.Registry.CreateObjectType<Circle>();
        s.Registry.CreateObjectType<Rectangle>();

        var tason1 = "Rectangle({Width:10,Height:20})";
        var tason2 = "Circle({Radius:50,Type:'circle'})";

        var rect = new Rectangle { Width = 10, Height = 20 };
        var circle = new Circle { Radius = 50 };

        Assert.Multiple(() =>
        {
            Assert.That(s.Deserialize<ShapeBase>(tason1), Is.EqualTo(rect));
            Assert.That(s.Deserialize<ShapeBase>(tason2), Is.EqualTo(circle));
        });

        var list = new List<ShapeBase> { rect, circle };
        var tason = $"[{tason1},{tason2}]";

        Assert.That(s.Deserialize<List<ShapeBase>>(tason), Is.EqualTo(list));
    }
}
