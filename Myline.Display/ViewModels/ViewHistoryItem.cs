namespace Myline.Display.ViewModels;

public sealed record ViewHistoryItem
{
	public required string FormattedTimestamp { get; init; }
	public required string Type { get; init; }
	public required string Site { get; init; }
	public required string Context { get; init; }
	public required string Description { get; init; }
}
