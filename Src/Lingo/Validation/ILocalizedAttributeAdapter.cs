#nullable enable

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bang.Lingo.Validation;

public interface ILocalizedAttributeAdapter
{
	String FormatErrorMessage(String message, ModelValidationContextBase validationContext);
}
