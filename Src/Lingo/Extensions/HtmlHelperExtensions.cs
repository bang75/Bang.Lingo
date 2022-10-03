#nullable enable

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bang.Lingo.Extensions;

public static class HtmlHelperExtensions
{
	public static IEnumerable<SelectListItem> GetEnumSelectList(this IHtmlHelper htmlHelper, Translator i18n, Type enumType)
	{
		return htmlHelper.GetEnumSelectList(enumType).Select(i => new SelectListItem(i18n.TranslateIfHashed(i.Text), i.Value));
	}

	public static IEnumerable<SelectListItem> GetEnumSelectList<TEnum>(this IHtmlHelper htmlHelper, Translator i18n) where TEnum : struct
	{
		return htmlHelper.GetEnumSelectList<TEnum>().Select(i => new SelectListItem(i18n.TranslateIfHashed(i.Text), i.Value));
	}

}