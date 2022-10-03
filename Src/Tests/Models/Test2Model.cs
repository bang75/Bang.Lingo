using Bang.Lingo;

namespace LingoTests.Models;

[LingoPrefix("Models.Test2")]
public class Test2Model : BaseModel
{
	public int Id { get; set; }

	[LingoPrefix(".Fields.SpecialName")]
	public string? Name { get; set; }

	[LingoPrefix("Fields.Email")]
	public string? Email { get; set; }

	public string? Phone { get; set; }
}
