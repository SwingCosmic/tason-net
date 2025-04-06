using Microsoft.Net.Http.Headers;
using System.Text;
using TASON;

namespace Microsoft.AspNetCore.Mvc.Formatters;

/// <summary>
/// 实现TASON格式响应输入
/// </summary>
public class TasonInputFormatter : TextInputFormatter
{
    MvcTasonOptions options;
    MvcOptions mvcOptions;
    public TasonInputFormatter(MvcTasonOptions options, MvcOptions mvcOptions)
    {
        this.options = options;
        this.mvcOptions = mvcOptions;

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(TasonConstants.MimeType));
        SupportedEncodings.Add(Encoding.UTF8);
    }

    /// <inheritdoc/>
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        if (encoding.EncodingName != Encoding.UTF8.EncodingName)
        {
            throw new NotSupportedException($"Encoding {encoding.EncodingName} is not supported");
        }

        var readStream = context.HttpContext.Request.Body;
        var type = context.ModelType;

        using (var streamReader = context.ReaderFactory(readStream, encoding))
        {
            var serializer = new TasonSerializer(options.SerializerOptions, options.TypeRegistry);
            var tason = await streamReader.ReadToEndAsync();
            object? model = serializer.Deserialize(tason, type);
            return InputFormatterResult.Success(model);
        }

    }
}
