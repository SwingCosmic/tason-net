using Microsoft.Net.Http.Headers;
using System.Reflection;
using System.Text;
using TASON;

namespace Microsoft.AspNetCore.Mvc.Formatters;
/// <summary>
/// 实现TASON格式响应输出
/// </summary>
public class TasonOutputFormatter : TextOutputFormatter
{
    MvcTasonOptions options;
    MvcOptions mvcOptions;
    public TasonOutputFormatter(MvcTasonOptions options, MvcOptions mvcOptions)
    {
        this.options = options;
        this.mvcOptions = mvcOptions;

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(TasonConstants.MimeType));
        SupportedEncodings.Add(Encoding.UTF8);
    }

    /// <inheritdoc/>
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        if (selectedEncoding.EncodingName != Encoding.UTF8.EncodingName)
        {
            throw new NotSupportedException($"Encoding {selectedEncoding.EncodingName} is not supported");
        }

        var response = context.HttpContext.Response;
        var data = context.Object;

        context.ContentType = TasonConstants.MimeType;

        await using (var writer = context.WriterFactory(response.Body, selectedEncoding))
        {
            var serializer = new TasonSerializer(options.SerializerOptions, options.TypeRegistry);
            await writer.WriteAsync(serializer.Serialize(data));
            await writer.FlushAsync();
        }
    }
}
