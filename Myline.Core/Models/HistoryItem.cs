using Myline.Core.Utilities.Extensions;

namespace Myline.Core.Models;

public sealed record HistoryItem
{
	public required Timestamp Timestamp { get; init; }

	public required string Site { get; init; }

	public required string Description
	{
		get;
		init => field = value.RegexReplace(@"[\t\r\n]", " ");
	}
}
