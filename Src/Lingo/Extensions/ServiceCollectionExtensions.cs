#nullable enable

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

using Bang.Lingo.Validation;

namespace Bang.Lingo.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLingo(this IServiceCollection services, Action<Lingo>? setup = null)
	{
		return services.AddLingo(config: null, setup);
	}

	public static IServiceCollection AddLingo(this IServiceCollection services, Action<LingoOptions>? config, Action<Lingo>? setup = null)
	{
		services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();

		services.AddHttpContextAccessor();

		if(config != null)
		{
			services.Configure(config);
		}

		services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();
			
		services.AddSingleton(sp =>
		{
			var lingo = new Lingo();

			setup?.Invoke(lingo);

			lingo.Load();

			return lingo;
		});


		services.AddTransient(sp =>
		{
			var prefix = sp.GetService<IHttpContextAccessor>()?.HttpContext?.Items["Lingo.Prefix"] as String;

			return sp.GetRequiredService<Lingo>().GetTranslator(Thread.CurrentThread.CurrentUICulture.Name, prefix);
		});

		return services;
	}
}