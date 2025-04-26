namespace TASON.Test;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TASON;
using TASON.Types;
using TASON.Types.SystemTextJson;
using TASON.Types.NewtonsoftJson;
using System.Text.Json;
using Newtonsoft.Json;
using TASON.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public class ParseTypeTest
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
    public void Date()
    {
        var s = TasonSerializer.Default;

        var date = s.Deserialize<DateTime>("Date('2020-01-15T09:00:00.666Z')");
        Assert.That(date, Is.EqualTo(new DateTime(2020, 1, 15, 9, 0, 0, 666, DateTimeKind.Utc)));
        
        var date2 = s.Deserialize<DateTime>("Date('2024-02-10 00:00:00-07:00')");
        date2 = date2.ToUniversalTime();
        Assert.That(date2, Is.EqualTo(new DateTimeOffset(2024, 2, 10, 0, 0, 0, TimeSpan.FromHours(-7)).UtcDateTime));

        var date3 = s.Deserialize<DateTime>("Date('2025-03-31 09:40:00')");
        Assert.That(date3, Is.EqualTo(new DateTime(2025, 3, 31, 9, 40, 0, DateTimeKind.Local)));

        var offset = DateTimeOffset.UtcNow;
        var timestamp = s.Deserialize<Timestamp>($"Timestamp('{offset.Millisecond + 1000}')");
        Assert.That(timestamp, Is.EqualTo(new Timestamp(offset.Millisecond) + 1000));

        var dateOnly = s.Deserialize<DateOnly>("DateOnly('2025-03-31')")!;
        Assert.That(dateOnly, Is.EqualTo(new DateOnly(2025, 3, 31)));

        var timeOnly = s.Deserialize<TimeOnly>("TimeOnly('13:40:59.999')");
        Assert.That(timeOnly, Is.EqualTo(new TimeOnly(13, 40, 59, 999)));
    }    
    
    [Test]
    public void Regex()
    {
        var s = TasonSerializer.Default;

        var reg = s.Deserialize<Regex>("RegExp('/([A-Z]+)\\\\1/mi')");
        var expect = new Regex("([A-Z]+)\\1", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Assert.That(RegexComparer.ToString(reg!), Is.EqualTo(RegexComparer.ToString(expect)));
    }

    [Test]
    public void JSON()
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
}
