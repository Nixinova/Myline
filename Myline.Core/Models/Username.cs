namespace Myline.Core.Models;

public class Username
{
	public string Value { get; init; }

	public Username(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new FormatException($"Invalid username: {value}");
		}
		Value = value.Trim();
	}
}
