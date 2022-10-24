#nullable enable

using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Monotype.Lingo.Extensions;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseLingo(this IApplicationBuilder app)
	{
		var lingo = app.ApplicationServices.GetService<Lingo>();

		if(lingo == null)
		{
			throw new InvalidOperationException(
				"Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddLingo' in the application startup code.");
		}

		lingo.Load();

		var lingoOptions = app.ApplicationServices.GetService<IOptions<LingoOptions>>();

		if(lingoOptions?.Value?.MapEndPoints == true && app is IEndpointRouteBuilder endPoints)
		{
			var getTranslations = (String? language, String? prefix, Lingo lingo) =>
			{
				var translator = lingo.GetTranslator(language);
				var translations = translator.Dictionary.GetTranslations(prefix, trimPrefix: false);

				var json = JsonSerializer.Serialize(translations.ToDictionary(e => e.Key.ToLowerInvariant(), e => e.Value.IsHtml ? e.Value.HtmlValue : e.Value.Value));

				return Results.Content($"/* Lingo. Language: {translator.Language}. Prefix: {prefix} */\nvar i18n = i18n || []; i18n = Object.assign({{}}, i18n || {{}}, {json});", "text/javascript");
			};

			endPoints.MapGet("/js/i18n.js", getTranslations);
			endPoints.MapGet("/js/i18n.{language}.js", getTranslations);
		}

		return app;
	}
}