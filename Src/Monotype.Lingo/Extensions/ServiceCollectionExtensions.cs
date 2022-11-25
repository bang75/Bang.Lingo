#nullable enable

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc.DataAnnotations;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

using Monotype.Localization.Validation;

namespace Monotype.Localization.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLingo(this IServiceCollection services, Action<LingoOptions>? config = null)
	{
		services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();

		services.AddHttpContextAccessor();

		services.ConfigureOptions<ConfigureMvcOptions>();


		// Lingo Options
		services.ConfigureOptions<ConfigureLingoOptions>();

		if(config != null)
		{
			services.Configure(config);
		}

		services.AddSingleton<Lingo>((sp) => new Lingo(sp.GetRequiredService<IOptions<LingoOptions>>()));


		// Register default translator
		services.AddTransient(sp =>
		{
			var prefix = sp.GetService<IHttpContextAccessor>()?
				.HttpContext?.Items["Lingo.Prefix"] as String;

			return sp.GetRequiredService<Lingo>()
				.GetTranslator(Thread.CurrentThread.CurrentUICulture.Name, prefix);
		});

		return services;
	}
}