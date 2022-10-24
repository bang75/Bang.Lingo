#nullable enable

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Monotype.Lingo.Validation;

public interface ILocalizedAttributeAdapter
{
	String FormatErrorMessage(String message, ModelValidationContextBase validationContext);
}
