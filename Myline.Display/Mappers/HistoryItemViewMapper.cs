using Myline.Core.Models;
using Myline.Display.ViewModels;

namespace Myline.Display.Mappers;

public static class HistoryItemViewMapper
{
	public static IReadOnlyCollection<ViewHistoryItem> MapToViewModel(IReadOnlyCollection<HistoryItem> items)
	{
		return items.Select(item =>
			new ViewHistoryItem
			{
				Type = item.Type.ToString(),
				FormattedTimestamp = CreateTimestamp(item.Timestamp),
				Title = item.Title,
			}
		).ToList();
	}

	private static string CreateTimestamp(Timestamp timestamp)
	{
		switch (timestamp.Precision)
		{
			case TimestampPrecision.Year:
				return timestamp.Time.Year.ToString();
			case TimestampPrecision.Month:
				return timestamp.Time.ToString("MMM yyyy");
			case TimestampPrecision.Day:
				return timestamp.Time.ToString("dd MMM yyyy");
			case TimestampPrecision.Hour:
				return timestamp.Time.ToString("dd MMM yyyy, ha");
			case TimestampPrecision.Minute:
				return timestamp.Time.ToString("dd MMM yyyy, h:mma");
			case TimestampPrecision.Second:
				return timestamp.Time.ToString("dd MMM yyyy, h:mm:ssa");
			default:
				return "";
		}
	}
}
