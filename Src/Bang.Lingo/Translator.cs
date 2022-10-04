#nullable enable

using Microsoft.AspNetCore.Html;

namespace Bang.Lingo;

public class Translator
{
	// Properties
	public TranslationDictionary Dictionary { get; init; }

	public String Language => this.Dictionary.Language;

	public String? Prefix { get; set; }


	// Indexer
	public String this[String key] => this.Translate(key)!;

	public String this[String key, params Object?[] args] => this.TranslateFormat(key, args)!;


	// Constructors
	internal Translator(Lingo lingo, TranslationDictionary dictionary, String? prefix = null)
	{
		this.Lingo = lingo;
		this.Dictionary = dictionary;
		this.Prefix = prefix;
	}

	internal Translator(Translator translator, String? prefix = null) : this(translator.Lingo, translator.Dictionary, prefix)
	{
	}


	// Methods
	public Boolean ContainsKey(String? key)
	{
		key = key.UnPrefix("#");

		var fullKey = this.GetFullKey(key);

		return fullKey != null ? this.Dictionary.ContainsKey(fullKey) : false;
	}


	public String? Translate(String? key, Boolean nullIfNotExists = false, Boolean suppressHtml = false) => this.Translate(key, false, nullIfNotExists, suppressHtml);

	public HtmlString? HtmlTranslate(String? key, Boolean nullIfNotExists = false) => new HtmlString(this.Translate(key, false, nullIfNotExists, suppressHtml: false));


	public String? TranslateIfHashed(String? key, Boolean nullIfNotExists = false, Boolean suppressHtml = false) => this.Translate(key, true, nullIfNotExists, suppressHtml);


	public String? TranslateFormat(String? key, Object?[] args, Boolean nullIfNotExists, Boolean suppressHtml)
	{
		if(this.ContainsKey(key))
		{
			var translatedArgs = args.Select(a => a != null && a is String argKey ? this.TranslateIfHashed(argKey) : a).ToArray<Object?>();

			return String.Format(this.Translate(key, false, nullIfNotExists, suppressHtml) ?? String.Empty, translatedArgs);
		}
		else
		{
			return this.Translate(key, nullIfNotExists);
		}
	}

	public String? TranslateFormat(String? key, params Object?[] args) => this.TranslateFormat(key, args, suppressHtml: false, nullIfNotExists: false);

	public HtmlString HtmlTranslateFormat(String? key, Object?[] args, Boolean nullIfNotExists = false) => new HtmlString(this.TranslateFormat(key, args, nullIfNotExists, suppressHtml: false));

	public HtmlString HtmlTranslateFormat(String? key, params Object?[] args) => this.HtmlTranslateFormat(key, args, nullIfNotExists: false);



	#region Protected Area

	// Properties
	protected readonly Lingo Lingo;


	// Methods
	protected String? GetFullKey(String? key)
	{
		if(!this.Prefix.IsNullOrWhiteSpace())
		{
			var hashed = key?.StartsWith("#") == true;

			if(hashed)
			{
				key = key![1..];
			}

			if(key?.StartsWith('.') == true)
			{
				if(!this.Prefix.IsNullOrWhiteSpace())
				{
					key = this.Prefix + key;
				}
				else
				{
					key = key.TrimStart('.');
				}
			}

			key = (hashed ? "#" : null) + key;
		}

		return key;
	}


	protected String? Translate(String? key, Boolean onlyIfHashed, Boolean nullIfNotExists, Boolean suppressHtml)
	{
		var result = key;

		var fullKey = this.GetFullKey(key);

		var hashed = fullKey?.StartsWith("#") == true;

		if(hashed)
		{
			fullKey = fullKey![1..];
		}

		if(!fullKey.IsNullOrWhiteSpace())
		{
			if(hashed || !onlyIfHashed)
			{
				var entry = this.Dictionary.GetEntry(fullKey!);

				if(entry != null)
				{
					result = this.Lingo.ParseParams((!suppressHtml ? entry.HtmlValue : null) ?? entry.Value, this.Language);
				}
				else
				{
					result = nullIfNotExists ? null : this.Lingo.GetMissingText(this.Language, fullKey);
				}
			}
		}

		return result;
	}


	#endregion

}