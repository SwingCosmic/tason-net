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

}
