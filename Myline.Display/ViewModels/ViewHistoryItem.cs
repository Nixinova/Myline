namespace Myline.Display.ViewModels;

public sealed record ViewHistoryItem
{
	public required long Sortkey { get; init; }
	public required string FormattedTimestamp { get; init; }
	public required string Site { get; init; }
	public required string Description { get; init; }
}
