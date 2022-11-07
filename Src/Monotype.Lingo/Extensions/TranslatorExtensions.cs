#nullable enable

namespace Monotype.Localization.Extensions;

public static class TranslatorExtensions
{
	public static Translator Prefixed(this Translator translator, String? prefix) => new Translator(translator, prefix);

	public static String? GetDisplayName(this Translator translator, Enum? enumItem) => enumItem.GetDisplayName(translator);

}