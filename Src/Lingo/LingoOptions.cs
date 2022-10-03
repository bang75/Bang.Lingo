#nullable enable

namespace Bang.Lingo;

public class LingoOptions
{
	public String MissingItemText { get; set; } = "[{language}] #{key}";

	public Dictionary<String, Func<String, String, String>> Parameters { get; set; } = new Dictionary<String, Func<String, String, String>>(StringComparer.OrdinalIgnoreCase);

	public Dictionary<Type, String> BasePrefixes { get; set; } = new Dictionary<Type, String>();
	
}