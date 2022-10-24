#nullable enable

using System.Globalization;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Microsoft.Extensions.Localization;

namespace Monotype.Lingo.Validation;

public class LocalizedMinLengthAttributeAdapter : LocalizedAttributeAdapter<MinLengthAttribute>
{
    public LocalizedMinLengthAttributeAdapter(MinLengthAttribute attribute, IStringLocalizer? stringLocalizer) : base(attribute, stringLocalizer)
    {
        this.Length = Convert.ToString(Attribute.Length, CultureInfo.InvariantCulture) ?? "";
    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        if(context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");

        MergeAttribute(context.Attributes, "data-val-minlength", this.GetErrorMessage(context));
        MergeAttribute(context.Attributes, "data-val-minlength-min", this.Length);
    }


    public override string FormatErrorMessage(String message, ModelValidationContextBase validationContext)
    {
        return String.Format(message, validationContext.ModelMetadata.GetDisplayName(), Attribute.Length);
    }



    #region Protected Area

    private readonly String Length;

    #endregion

}