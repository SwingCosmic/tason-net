namespace TASON.Test;

using TASON;
using System.Collections.Generic;
using System.Drawing;
using TASON.Serialization;

class PointType : TasonObjectType<Point>
{
    public override object Deserialize(Dictionary<string, object?> dict, TasonSerializerOptions options)
    {
        var point = new Point();
        foreach (var (p, v) in dict)
        {
            if (p == nameof(Point.X)) point.X = v is int x ? x : default;
            if (p == nameof(Point.Y)) point.Y = v is int y ? y : default;
        }
        return point;
    }
}