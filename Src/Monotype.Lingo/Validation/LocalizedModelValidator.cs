#nullable enable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Monotype.Lingo.Validation;

// Bara för backend validering
public class LocalizedModelValidator : IModelValidator
{
    // Constructors
    public LocalizedModelValidator(ValidationAttribute attr, IModelValidator baseValidator, IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
    {
        Attribute = attr;
        BaseValidator = baseValidator;
        ValidationAttributeAdapterProvider = validationAttributeAdapterProvider;
    }


    // Methods
    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
    {
        var result = new List<ModelValidationResult>();

        foreach (var validation in BaseValidator.Validate(context))
        {
            var adapter = ValidationAttributeAdapterProvider.GetAttributeAdapter(Attribute, null);
            var message = adapter?.GetErrorMessage(context) ?? Attribute.ErrorMessage;

            result.Add(new ModelValidationResult(validation.MemberName, message ?? $"Validation error [{Attribute.GetType().Name}]."));
        }

        return result;
    }


    #region Protected Area

    private readonly ValidationAttribute Attribute;
    private readonly IModelValidator BaseValidator;
    private readonly IValidationAttributeAdapterProvider ValidationAttributeAdapterProvider;

    #endregion

}