namespace Myline.Display.ViewModels;

public sealed record ViewHistoryItem
{
	public required string Type { get; init; }
	public required string FormattedTimestamp { get; init; }
	public required string Title { get; init; }
}
