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
				Sortkey = item.Timestamp.Time.Ticks,
				FormattedTimestamp = CreateTimestamp(item.Timestamp),
				Type = item.Type.ToString(),
				Site = item.Site,
				Context = item.Context,
				Description = item.Description,
			}
		).ToList();
	}

	private static string CreateTimestamp(Timestamp timestamp)
	{
		var tzOffset = TimeZoneInfo.Local.GetUtcOffset(timestamp.Time);
		var time = DateTime.SpecifyKind(timestamp.Time + tzOffset, DateTimeKind.Local);
		return timestamp.Precision switch
		{
			TimestampPrecision.Year => time.Year.ToString(),
			TimestampPrecision.Month => time.ToString("yyyy-MMM"),
			TimestampPrecision.Day => time.ToString("yyyy-MMM-dd"),
			TimestampPrecision.Hour => time.ToString("yyyy-MMM-dd htt"),
			TimestampPrecision.Minute => time.ToString("yyyy-MMM-dd HH:mm"),
			TimestampPrecision.Second => time.ToString("yyyy-MMM-dd HH:mm:ss"),
			_ => ""
		};
	}
}
