#nullable enable

namespace Monotype.Lingo;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class LingoPrefixAttribute : Attribute
{
	public String Prefix { get; set; }

	public LingoPrefixAttribute(String prefix)
	{
		this.Prefix = prefix;
	}
}
