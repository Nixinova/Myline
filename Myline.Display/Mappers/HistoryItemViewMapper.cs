using System.Globalization;
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
				Site = item.Site,
				Description = item.Description,
			}
		).ToList();
	}

	private static string CreateTimestamp(Timestamp timestamp)
	{
		var tzOffset = TimeZoneInfo.Local.GetUtcOffset(timestamp.Time);
		var time = DateTime.SpecifyKind(timestamp.Time + tzOffset, DateTimeKind.Local);
		return (timestamp.Precision switch
		{
			TimestampPrecision.Year => time.Year.ToString(),
			TimestampPrecision.Month => time.ToString("yyyy-MMM", CultureInfo.InvariantCulture),
			TimestampPrecision.Day => time.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
			TimestampPrecision.Hour => time.ToString("yyyy-MMM-dd htt", CultureInfo.InvariantCulture),
			TimestampPrecision.Minute => time.ToString("yyyy-MMM-dd HH:mm", CultureInfo.InvariantCulture),
			TimestampPrecision.Second => time.ToString("yyyy-MMM-dd HH:mm:ss", CultureInfo.InvariantCulture),
			_ => ""
		});
	}
}
