#nullable enable

namespace Monotype.Localization.Extensions;

public static class TranslatorExtensions
{
	public static Translator Prefixed(this Translator translator, String? prefix)
	{
		return new Translator(translator, prefix);
	}

}