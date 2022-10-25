#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

using Microsoft.Extensions.Options;

using Monotype.Lingo.Validation;

namespace Monotype.Lingo;

internal class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
{
	// Constructors
	public ConfigureMvcOptions(Lingo lingo, IOptions<LingoOptions> options, IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
	{
		this.Lingo = lingo;
		this.Options = options.Value;
		this.ValidationAttributeAdapterProvider = validationAttributeAdapterProvider;
	}

	public void Configure(MvcOptions options)
	{
		options.Filters.Add(new LingoFilter());

		// Display Localization
		options.ModelMetadataDetailsProviders.Add(new DisplayMetadataProvider(this.Lingo, this.Options.MissingTranslationMode));

		// Validator Providers
		options.ModelValidatorProviders.Add(new LocalizedModelValidatorProvider(this.ValidationAttributeAdapterProvider));
	}



	#region Protected Area

	private readonly Lingo Lingo;
	private readonly LingoOptions Options;
	private readonly IValidationAttributeAdapterProvider ValidationAttributeAdapterProvider;

	#endregion

}