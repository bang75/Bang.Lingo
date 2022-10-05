#nullable enable

using System.Globalization;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Microsoft.Extensions.Localization;

namespace Bang.Lingo.Validation;

public class LocalizedMaxLengthAttributeAdapter : LocalizedAttributeAdapter<MaxLengthAttribute>
{
	public LocalizedMaxLengthAttributeAdapter(MaxLengthAttribute attribute, IStringLocalizer? stringLocalizer) : base(attribute, stringLocalizer)
	{
		this.Length = Convert.ToString(Attribute.Length, CultureInfo.InvariantCulture);
	}

	public override void AddValidation(ClientModelValidationContext context)
	{
		if(context == null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		MergeAttribute(context.Attributes, "data-val", "true");

		MergeAttribute(context.Attributes, "data-val-maxlength", this.GetErrorMessage(context));
		MergeAttribute(context.Attributes, "data-val-maxlength-max", this.Length);
	}


	public override String FormatErrorMessage(String message, ModelValidationContextBase validationContext)
	{
		return String.Format(message, validationContext.ModelMetadata.GetDisplayName(), this.Attribute.Length);
	}



	#region Protected Area

	private readonly String Length;

	#endregion

}
