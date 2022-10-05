#nullable enable

namespace Bang.Lingo;

public class LingoOptions
{
	public Boolean Debug { get; set; } = true;

	public String FieldsPrefix { get; set; } = "Fields";

	public String MissingItemText { get; set; } = "[{language}] #{key}";

	public Dictionary<String, Func<String, String, String>> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);

	public Dictionary<Type, Func<Type, String?>> BasePrefixes { get; set; } = new();

	public event Action<Lingo>? LoadTranslations;


	public LingoOptions()
	{
		this.AddBasePrefix<Enum>("Enums");
	}

	public void AddParameter(String name, Func<String, String, String> value)
	{
		this.Parameters[name] = value;
	}

	public void AddBasePrefix(Type type, Func<Type, String?> prefix)
	{
		this.BasePrefixes[type] = prefix;
	}

	public void AddBasePrefix(Type type, String prefix) => this.AddBasePrefix(type, (obj) => prefix);

	public void AddBasePrefix<T>(String prefix) => this.AddBasePrefix(typeof(T), prefix);

	public void AddBasePrefix<T>(Func<Type, String?> prefix) => this.AddBasePrefix(typeof(T), prefix);


	public Action<Lingo>? GetLoadTranslations() => this.LoadTranslations;
}