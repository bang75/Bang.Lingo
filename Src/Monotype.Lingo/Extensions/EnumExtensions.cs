using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Monotype.Localization.Extensions;

public static class EnumExtensions
{
	public static String? GetDisplayName(this Enum? enumItem, Translator? translator = null)
	{
		var enumType = enumItem?.GetType();
		var enumName = enumItem?.ToString();

		var displayName = enumType?
			.GetMember(enumName!)
			.FirstOrDefault()?
			.GetCustomAttribute<DisplayAttribute>()?
			.GetName()
			.TrimToNull();

		var prefixAttr = enumType?.GetCustomAttribute<LingoPrefixAttribute>();

		if(prefixAttr != null)
		{
			translator = translator?.Prefixed(prefixAttr.Prefix);

			if(displayName == null)
			{
				displayName = $"#.{enumName}.DisplayName";
			}
		}

		if(displayName?.StartsWith('#') == true)
		{
			displayName = translator?.TranslateIfHashed(displayName, nullIfNotExists: true);
		}

		return displayName ?? enumName.ToReadable();
	}
}
