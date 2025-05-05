namespace TASON.Test;

using TASON;
using TASON.Types.SystemTextJson;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using TASON.Serialization;

public class ParseGenericTest
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

    [Test]
    public void StructTest()
    {
        var s = TasonSerializer.Default.Clone();
        s.Registry.RegisterType("Point", new PointType());

        Assert.That(s.Deserialize<Point[]>("[Point({X:1,Y:2}), Point({X:3,Y:4})]"),
            Is.EqualTo(new Point[] { new (1, 2), new (3, 4) }));
    }


    [Test]
    public void PublicFieldTest() 
    {
        var tason = "{PublicField:'foo',PublicProperty:666}";
        var tason2 = $"PublicFieldClass({tason})";

        var s = TasonSerializer.Default.Clone();
        s.Options.AllowFields = true;
        Assert.That(s.Deserialize<PublicFieldClass>(tason),
            Is.EqualTo(new PublicFieldClass() { PublicField = "foo", PublicProperty = 666 }));

        s.Registry.CreateObjectType<PublicFieldClass>();
        Assert.That(s.Deserialize<PublicFieldClass>(tason2),
            Is.EqualTo(new PublicFieldClass() { PublicField = "foo", PublicProperty = 666 }));



        var s2 = TasonSerializer.Default.Clone();
        s2.Options.AllowFields = false;
        Assert.That(s2.Deserialize<PublicFieldClass>(tason),
            Is.EqualTo(new PublicFieldClass() { PublicProperty = 666 }));

        s2.Registry.CreateObjectType<PublicFieldClass>();
        Assert.That(s2.Deserialize<PublicFieldClass>(tason2),
            Is.EqualTo(new PublicFieldClass() { PublicProperty = 666 }));
    }


    [Test]
    public void CustomMetadata()
    {
        var s = TasonSerializer.Default.Clone();
        s.Registry.RegisterType(
            nameof(ClassWithPrivateField),
            new TasonObjectType<ClassWithPrivateField>(),
            new ClassWithPrivateField.Metadata());
        s.Options.AllowFields = true;

        var obj = new ClassWithPrivateField();
        obj.UpdateValue(666);

        Assert.That(s.Deserialize<ClassWithPrivateField>("ClassWithPrivateField({m_serializeField:666})"), Is.EqualTo(obj));
    }
}
