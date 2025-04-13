namespace TASON.Test;

using System.Text.RegularExpressions;
using TASON;
using TASON.Types.SystemTextJson;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class ParseGenericTest
{
    JsonSerializerOptions options = null!;

    class TestList : List<string>
    {
        public TestList() : base()
        { 
        }

        public TestList(IEnumerable<string> items) : base(items) { }
    }

    [SetUp]
    public void Setup()
    {
        options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        TasonSerializer.Default.Registry
            .AddSystemTextJson(options);
    }

    record class A
    {
        public int X { get; set; }
        public int Y { get; set; }
    }


    class ADict : Dictionary<A, int>
    {

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

        s.Registry.CreateObjectType(typeof(A));
        Assert.That(s.Deserialize<A>("{X:1, Y:2, }"), Is.EqualTo(new A() { X = 1, Y = 2 }));


    }    
    
    
    [Test]
    public void DictionaryTest()
    {
        var pairs = "[A({X:1,Y:2}),1],[A({X:2,Y:4}),2]";
        var tason = $"Dictionary({{keyValuePairs:[{pairs}]}})";

        var s = TasonSerializer.Default.Clone();
        s.Options.UseBuiltinDictionary = true;
        s.Registry.CreateObjectType(typeof(A));

        Assert.That(s.Deserialize<ADict>(tason), 
            Is.EqualTo(new ADict() 
            {
                [new A { X = 1,Y = 2 }] = 1,
                [new A { X = 2,Y = 4 }] = 2,
            }));


        var s2 = TasonSerializer.Default.Clone();
        s2.Options.UseBuiltinDictionary = false;
        s2.Registry.CreateObjectType(typeof(A));

        Assert.Throws<ArgumentException>(() => s2.Deserialize<ADict>(tason));
    }

}
