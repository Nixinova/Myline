namespace Myline.Core.Models;

public sealed record HistoryItem
{
	public required HistoryType Type { get; init; }
	public required Timestamp Timestamp { get; init; }
	public required string Title { get; init; }
}
