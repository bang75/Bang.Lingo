#nullable enable

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Monotype.Localization.Validation;

public interface ILocalizedAttributeAdapter
{
	String FormatErrorMessage(String message, ModelValidationContextBase validationContext);
}
