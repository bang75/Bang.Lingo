#nullable enable

using System.Reflection;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Monotype.Lingo.Validation;

// Bara för klientvalidering / unobtrusive
public abstract class LocalizedAttributeAdapter<T> : AttributeAdapterBase<T>, ILocalizedAttributeAdapter where T : ValidationAttribute
{
	// Constructors
	public LocalizedAttributeAdapter(T attribute, IStringLocalizer? stringLocalizer) : base(attribute, stringLocalizer)
	{
		var validationAttrType = typeof(ValidationAttribute);

		this.ErrorMsgField = validationAttrType.GetField("_errorMessage", BindingFlags.Instance | BindingFlags.NonPublic);
		this.DefaultErrorMsgField = validationAttrType.GetField("_defaultErrorMessage", BindingFlags.Instance | BindingFlags.NonPublic);
	}

	// Methods
	public override String GetErrorMessage(ModelValidationContextBase validationContext)
	{
		ArgumentNullException.ThrowIfNull(validationContext, nameof(validationContext));
		ArgumentNullException.ThrowIfNull(validationContext.ModelMetadata, nameof(validationContext.ModelMetadata));

		var i18n = validationContext.ActionContext.HttpContext.RequestServices.GetRequiredService<Translator>();
		var attrName = this.Attribute.GetType().Name.UnSuffix(nameof(Attribute));

		var message = this.ErrorMsgField?.GetValue(this.Attribute)?.ToString();

		if(message == null)
		{
			if(validationContext.ModelMetadata is DefaultModelMetadata modelMetadata)
			{
				var prefixAttr = modelMetadata.Attributes.Attributes.OfType<LingoPrefixAttribute>().FirstOrDefault();

				var prefix = prefixAttr?.Prefix ?? modelMetadata.Name.Prefix(".");

				if(prefix.IsPrefixed("."))
				{
					if(modelMetadata.ContainerType != null)
					{
						var containerPrefix = modelMetadata.ContainerType.GetCustomAttribute<LingoPrefixAttribute>(true)?.Prefix;

						if(!containerPrefix.IsNullOrWhiteSpace())
						{
							prefix = containerPrefix + ".Fields" + prefix;
						}
					}
					else
					{
						prefix = null;
					}
				}

				if(prefix != null)
				{
					message = "#" + prefix + "Errors." + attrName;
				}
				else
				{
					message = "#Errors.Validation." + attrName;
				}
			}
		}

		var key = message;

		if(key.IsPrefixed("#"))
		{
			var defaultMessageKey = "Errors.Validation." + attrName;

			message = i18n.Translate(key, nullIfNotExists: true);

			if(message == null && key != defaultMessageKey)
			{
				message = i18n.Translate(defaultMessageKey, nullIfNotExists: true);
			}
		}

		if(message.IsNullOrWhiteSpace())
		{
			message = this.DefaultErrorMsgField?.GetValue(this.Attribute)?.ToString();
		}

		return message != null ? this.FormatErrorMessage(message, validationContext) : $"Validation error [{attrName}].";
	}


	public virtual String FormatErrorMessage(String message, ModelValidationContextBase validationContext)
	{
		return String.Format(message, validationContext.ModelMetadata.GetDisplayName());
	}



	#region Protected Area

	private readonly FieldInfo? ErrorMsgField;

	private readonly FieldInfo? DefaultErrorMsgField;

	#endregion

}