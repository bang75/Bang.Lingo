#nullable enable

namespace Monotype.Localization;

public class TranslationEntry
{
	// Properties
	public String Key { get; }

	public String Value { get; }

	public String? HtmlValue { get; }

	public Boolean IsHtml => this.HtmlValue != null;


	// Constructors
	public TranslationEntry(String key, String value, String? htmlValue)
	{
		this.Key = key;
		this.Value = value;
		this.HtmlValue = htmlValue;
	}

}	