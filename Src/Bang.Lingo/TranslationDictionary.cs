#nullable enable

using System.Collections.Immutable;

using Markdig;

namespace Bang.Lingo;

public class TranslationDictionary
{
	// Properties
	public String Language { get; init; }

	public Int32 NoOfTranslations => this.Translations.Count();


	// Constructors
	internal TranslationDictionary(String language)
	{
		this.Language = language;
	}


	// Methods
	public Boolean ContainsKey(String key) => this.Translations.ContainsKey(key);

	public void Add(String key, String? text, TextFormat textFormat = TextFormat.Plain, Boolean replaceIfExists = true)
	{
		var exists = this.ContainsKey(key);

		if(!exists || replaceIfExists)
		{
			String? htmlText = null;

			lock(this.Translations)
			{
				if(exists)
				{
					this.Translations = this.Translations.Remove(key);
				}

				if(!text.IsNullOrWhiteSpace())
				{
					switch(textFormat)
					{
						case TextFormat.Markup:
							htmlText = ParseMarkup(text, removeParagraph: true);
							break;

						case TextFormat.ParagraphedMarkup:
							htmlText = ParseMarkup(text, removeParagraph: false);
							break;
					}

					if(textFormat != TextFormat.Plain)
					{
						text = Markdown.ToPlainText(text!);
					}
				}

				var entry = new TranslationEntry(key, text ?? String.Empty, htmlText);

				this.Translations = this.Translations.Add(key, entry);
			}
		}
	}

	public TranslationEntry? GetEntry(String key) => this.Translations.TryGetValue(key, out var entry) ? entry : null;


	public IDictionary<String, TranslationEntry> GetTranslations(String? prefix = null, Boolean trimPrefix = true)
	{
		var translations = this.Translations.AsEnumerable();

		if(!prefix.IsNullOrWhiteSpace())
		{
			prefix = prefix.Suffix(".");

			translations = translations.Where(kvp => kvp.Key.StartsWith(prefix!, StringComparison.OrdinalIgnoreCase));
		}

		return translations.ToDictionary(kvp => trimPrefix && !prefix.IsNullOrWhiteSpace() ? kvp.Key.UnPrefix(prefix!)! : kvp.Key, kvp => kvp.Value);
	}



	#region Protected Area

	// Static
	protected static String? ParseMarkup(String? markup, Boolean removeParagraph = false)
	{
		String? html = null;

		if(!markup.IsNullOrWhiteSpace())
		{
			markup = String.Join('\n', markup!.Split('\n').Select(r => r.Trim()));

			html = Markdown.ToHtml(markup);

			if(html != null && removeParagraph)
			{
				html = html.Replace("<p>", "", StringComparison.OrdinalIgnoreCase);
				html = html.Replace("</p>", "", StringComparison.OrdinalIgnoreCase);
			}
		}

		return html;
	}


	// Properties
	protected ImmutableDictionary<String, TranslationEntry> Translations = ImmutableDictionary<String, TranslationEntry>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);


	#endregion
}