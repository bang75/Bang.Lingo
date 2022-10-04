#nullable enable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Bang.Lingo.Validation;

public class LocalizedRegularExpressionAttributeAdapter : LocalizedAttributeAdapter<RegularExpressionAttribute>
{
	public LocalizedRegularExpressionAttributeAdapter(RegularExpressionAttribute attribute, IStringLocalizer? stringLocalizer) : base(attribute, stringLocalizer!)
	{
	}

	public override void AddValidation(ClientModelValidationContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		MergeAttribute(context.Attributes, "data-val", "true");
		MergeAttribute(context.Attributes, "data-val-regex-pattern", this.Attribute.Pattern);
		MergeAttribute(context.Attributes, "data-val-regex", this.GetErrorMessage(context));
	}
}