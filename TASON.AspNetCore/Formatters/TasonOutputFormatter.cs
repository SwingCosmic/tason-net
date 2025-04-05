using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using TASON;

namespace Microsoft.AspNetCore.Mvc.Formatters;

public class TasonOutputFormatter : TextOutputFormatter
{
    public const string MimeType = "application/x-tason";

    MvcTasonOptions options;
    MvcOptions mvcOptions;
    public TasonOutputFormatter(MvcTasonOptions options, MvcOptions mvcOptions)
    {
        this.options = options;
        this.mvcOptions = mvcOptions;

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MimeType));
        SupportedEncodings.Add(Encoding.UTF8);
    }
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        if (selectedEncoding.EncodingName != Encoding.UTF8.EncodingName)
        {
            throw new NotSupportedException($"Encoding {selectedEncoding.EncodingName} is not supported");
        }

        var response = context.HttpContext.Response;
        var data = context.Object;

        context.ContentType = MimeType;

        await using (var writer = context.WriterFactory(response.Body, selectedEncoding))
        {
            var serializer = new TasonSerializer(options.SerializerOptions, options.TypeRegistry);
            await writer.WriteAsync(serializer.Serialize(data));
            await writer.FlushAsync();
        }
    }
}
