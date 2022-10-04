#nullable enable

namespace Bang.Lingo;

public class LingoOptions
{
	public String MissingItemText { get; set; } = "[{language}] #{key}";

	public Dictionary<String, Func<String, String, String>> Parameters { get; set; } = new Dictionary<String, Func<String, String, String>>(StringComparer.OrdinalIgnoreCase);

	public Dictionary<Type, String> BasePrefixes { get; set; } = new Dictionary<Type, String>();

	public event Action<Lingo>? LoadTranslations;


	public LingoOptions()
	{
		this.BasePrefixes.Add(typeof(Enum), "Enums");
	}

	public void AddParameter(String name, Func<String, String, String> value)
	{
		this.Parameters[name] = value;
	}

	public void AddBasePrefix<T>(String prefix) => this.AddBasePrefix(typeof(T), prefix);

	public void AddBasePrefix(Type type, String prefix)
	{
		this.BasePrefixes[type] = prefix;
	}

	public Action<Lingo>? GetLoadTranslations() => this.LoadTranslations;
}