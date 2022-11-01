#nullable enable

using System.ComponentModel.DataAnnotations;
using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Microsoft.Extensions.Localization;

namespace Monotype.Localization.Validation;

public class LocalizedRangeAttributeAdapter : LocalizedAttributeAdapter<RangeAttribute>
{
	public LocalizedRangeAttributeAdapter(RangeAttribute attribute, IStringLocalizer? stringLocalizer) : base(attribute, stringLocalizer)
	{
		this.Minimum = Convert.ToString(Attribute.Minimum, CultureInfo.InvariantCulture) ?? "";
		this.Maximum = Convert.ToString(Attribute.Maximum, CultureInfo.InvariantCulture) ?? "";
	}

	public override void AddValidation(ClientModelValidationContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		MergeAttribute(context.Attributes, "data-val", "true");

		MergeAttribute(context.Attributes, "data-val-range", this.GetErrorMessage(context));
		MergeAttribute(context.Attributes, "data-val-range-min", this.Minimum);
		MergeAttribute(context.Attributes, "data-val-range-max", this.Maximum);

	}

	public override String FormatErrorMessage(String message, ModelValidationContextBase validationContext)
	{
		var displayName = validationContext.ModelMetadata.GetDisplayName();

		return String.Format(message, displayName, this.Minimum, this.Maximum);
	}



	#region Protected Area

	private readonly String Maximum;
	private readonly String Minimum;

	#endregion

}