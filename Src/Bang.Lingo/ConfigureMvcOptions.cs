#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

using Microsoft.Extensions.Options;

using Bang.Lingo.Validation;

namespace Bang.Lingo;

internal class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
{
	// Constructors
	public ConfigureMvcOptions(Lingo lingo, IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
	{
		this.Lingo = lingo;
		this.ValidationAttributeAdapterProvider = validationAttributeAdapterProvider;
	}

	public void Configure(MvcOptions options)
	{
		options.Filters.Add(new LingoFilter());

		// Display Localization
		options.ModelMetadataDetailsProviders.Add(new DisplayMetadataProvider(this.Lingo));

		// Validator Providers
		options.ModelValidatorProviders.Add(new LocalizedModelValidatorProvider(this.ValidationAttributeAdapterProvider));
	}



	#region Protected Area

	private readonly Lingo Lingo;
	private readonly IValidationAttributeAdapterProvider ValidationAttributeAdapterProvider;

	#endregion

}