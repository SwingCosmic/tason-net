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

public class SerializeTypeTest
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
    public void RegExp()
    {
        var s = TasonSerializer.Default;

        var reg = new Regex("[a-z]+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Assert.That(s.Serialize(reg), Is.EqualTo("RegExp(\"/[a-z]+/imu\")"));
    }

    [Test]
    public void Date()
    {
        var s = TasonSerializer.Default;

        var date = new DateTime(2020, 1, 15, 9, 0, 0, 666, DateTimeKind.Utc);
        Assert.That(s.Serialize(date), Is.EqualTo("Date(\"2020-01-15T09:00:00.666Z\")"));

        var date3 = new DateTimeOffset(2025, 3, 31, 9, 40, 0, TimeSpan.FromHours(8)).DateTime;
        Assert.That(s.Serialize(date3), Is.EqualTo("Date(\"2025-03-31T01:40:00.000Z\")"));

        var offset = DateTimeOffset.UtcNow;
        var timestamp = new Timestamp(offset.Millisecond + 1000);
        Assert.That(s.Serialize(timestamp), Is.EqualTo($"Timestamp(\"{offset.Millisecond + 1000}\")"));

        var dateOnly = new DateOnly(2025, 3, 31);
        Assert.That(s.Serialize(dateOnly), Is.EqualTo("DateOnly(\"2025-03-31\")"));

        var timeOnly = new TimeOnly(13, 40, 59, 999);
        Assert.That(s.Serialize(timeOnly), Is.EqualTo("TimeOnly(\"13:40:59.999\")"));
    }

}
