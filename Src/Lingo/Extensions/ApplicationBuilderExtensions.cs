#nullable enable

using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bang.Lingo.Extensions;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseLingo(this IApplicationBuilder app)
	{
		if(app is IEndpointRouteBuilder endPoints)
		{
			var getTranslations = (String? prefix, Translator i18n) =>
			{
				var translations = i18n.Dictionary.GetTranslations(prefix, trimPrefix: false);

				var json = JsonSerializer.Serialize(translations.ToDictionary(e => e.Key.ToLowerInvariant(), e => e.Value.IsHtml ? e.Value.HtmlValue : e.Value.Value));

				return Results.Content($"var i18n = i18n || []; i18n = Object.assign({{}}, i18n || {{}}, {json});", "text/javascript");
			};

			endPoints.MapGet("/js/i18n.js", getTranslations);
			endPoints.MapGet("/js/i18n/{prefix}.js", getTranslations);
		}

		return app;
	}
}