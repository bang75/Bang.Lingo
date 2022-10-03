#nullable enable

using System.Collections.Immutable;

namespace Bang.Lingo;

public class Lingo
{
	// Events
	public event Action<Lingo>? LoadTranslations;


	// Properties
	public readonly String MissingItemText;

	public Dictionary<String, Func<String, String, String>> Parameters;



	// Constructors
	public Lingo(LingoOptions? options = null)
	{
		options = options ?? new LingoOptions();

		this.MissingItemText = options.MissingItemText;
		this.Parameters = options.Parameters;
		this.BasePrefixes = options.BasePrefixes;
	}


	// Methods
	public void Load()
	{
		lock(this.Dictionaries)
		{
			this.Dictionaries = this.Dictionaries.Clear();

			this.LoadTranslations?.Invoke(this);
		}
	}

	public String? GetBasePrefix(Type? type)
	{
		String? prefix = null;

		while(type != null)
		{
			if(this.BasePrefixes.TryGetValue(type, out var typePrefix))
			{
				prefix = typePrefix + prefix;

				if(!prefix.IsPrefixed("."))
				{
					break;
				}
			}

			type = type.BaseType;
		}

		return prefix;
	}

	public TranslationDictionary GetDictionary(String? language = null)
	{
		lock(this.Dictionaries)
		{
			if(language.IsNullOrWhiteSpace())
			{
				language = Thread.CurrentThread.CurrentUICulture.Name;
			}

			if(!this.Dictionaries.TryGetValue(language!, out var dictionary))
			{
				this.Dictionaries = this.Dictionaries.Add(language!, dictionary = new TranslationDictionary(language!));
			}

			return dictionary;
		}
	}

	public Translator GetTranslator(String? language = null, String? prefix = null)
	{
		lock(this.Dictionaries)
		{
			return new Translator(this, this.GetDictionary(language), prefix);
		}
	}


	#region Protected Area

	// Properties
	protected ImmutableDictionary<String, TranslationDictionary> Dictionaries = ImmutableDictionary<String, TranslationDictionary>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

	protected readonly Dictionary<Type, String> BasePrefixes;

	#endregion
}