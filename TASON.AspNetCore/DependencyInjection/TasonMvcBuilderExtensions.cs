
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using TASON;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for configuring MVC via an <see cref="IMvcBuilder"/>.
/// </summary>
public static class TasonMvcBuilderExtensions
{
    /// <summary>
    /// Configures features for <see cref="TasonSerializer"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/>.</returns>
    public static IMvcBuilder AddTason(this IMvcBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        AddServices(builder);
        return builder;
    }

    /// <summary>
    /// Configures features for <see cref="TasonSerializer"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="setupAction">Callback to configure <see cref="MvcTasonOptions"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/>.</returns>
    public static IMvcBuilder AddTason(this IMvcBuilder builder, Action<MvcTasonOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(setupAction);

        AddServices(builder);
        builder.Services.Configure(setupAction);
        return builder;
    }


    static void AddServices(IMvcBuilder builder)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, TasonMvcOptionsSetup>());
    }
    
}

internal sealed class TasonMvcOptionsSetup(IOptions<MvcTasonOptions> _options) : IConfigureOptions<MvcOptions>
{
    public void Configure(MvcOptions mvcOptions)
    {
        var options = _options.Value;
        mvcOptions.OutputFormatters.Add(new TasonOutputFormatter(options, mvcOptions));
        if (options.GetAutoRegisterObjectTypes is not null)
        {
            foreach (var type in options.GetAutoRegisterObjectTypes())
            {
                options.TypeRegistry.CreateObjectType(type);
            }
        }
    }
}