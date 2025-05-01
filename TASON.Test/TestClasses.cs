using TASON.Serialization;

namespace TASON.Test;

class TestList : List<string>
{
    public TestList() : base()
    { 
    }

    public TestList(IEnumerable<string> items) : base(items) { }
}

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

record class NullableClass
{
    public int? IntVal { get; set; }

    public DateTime? DateTimeVal { get; set; }
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


record class DynamicFieldClass
{
    [TasonExtraMember]
    public IDictionary<string, object?> DynamicFields { get; set; } = new Dictionary<string, object?>();


    public string NormalProperty { get; set; } = "";
}


abstract record class ShapeBase
{
    public abstract string Kind { get; }
}

record class Circle : ShapeBase
{
    public override string Kind => "circle";

    public double Radius { get; set; }
}

record class Rectangle : ShapeBase
{
    public override string Kind => "rectangle";

    public double Width { get; set; }
    public double Height { get; set; }
}

record class PublicFieldClass
{
    public string PublicField = "";

    public int PublicProperty { get; set; } = 1;

    int _privateField = 5;
}