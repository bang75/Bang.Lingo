#nullable enable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Microsoft.Extensions.Localization;

namespace Monotype.Lingo.Validation;

public class LocalizedRequiredAttributeAdapter : LocalizedAttributeAdapter<RequiredAttribute>
{
	public LocalizedRequiredAttributeAdapter(RequiredAttribute attribute, IStringLocalizer? stringLocalizer) : base(attribute, stringLocalizer)
	{
	}

	public override void AddValidation(ClientModelValidationContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		MergeAttribute(context.Attributes, "data-val", "true");
		MergeAttribute(context.Attributes, "data-val-required", this.GetErrorMessage(context));
	}
}