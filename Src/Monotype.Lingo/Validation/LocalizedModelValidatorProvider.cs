#nullable enable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Monotype.Lingo.Validation;

public class LocalizedModelValidatorProvider : IModelValidatorProvider
{
	// Constructors
	public LocalizedModelValidatorProvider(IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
	{
		ValidationAttributeAdapterProvider = validationAttributeAdapterProvider;
	}


	// Methods
	public void CreateValidators(ModelValidatorProviderContext context)
	{
		foreach(var validatorItem in context.Results)
		{
			if(validatorItem.Validator != null && validatorItem.ValidatorMetadata is ValidationAttribute attr)
			{
				validatorItem.Validator = new LocalizedModelValidator(attr, validatorItem.Validator, ValidationAttributeAdapterProvider);
			}
		}
	}


	#region Protected Area

	private readonly IValidationAttributeAdapterProvider ValidationAttributeAdapterProvider;

	#endregion

}
