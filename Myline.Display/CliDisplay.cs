using Myline.Core.Utilities.Extensions;
using Myline.Display.ViewModels;

namespace Myline.Display;

public static class CliDisplay
{
	public static void DisplayData(IReadOnlyCollection<ViewHistoryItem> items)
	{
		var maxTimestampLen = items.Select(x => x.FormattedTimestamp.Length).SafeMax();
		var maxSiteLen = items.Select(x => x.Site.Length).SafeMax();
		foreach (var item in items.OrderBy(x => x.Sortkey))
		{
			var line = string.Join(" ",
				item.FormattedTimestamp.PadRight(maxTimestampLen, ' ') + " ",
				CenterAlign(item.Site, maxSiteLen) + " ",
				item.Description
			);
			if (line.Length > Console.WindowWidth)
			{
				line = line[..(Console.WindowWidth - 1)] + "…";
			}

			Console.WriteLine(line);
		}
	}

	private static string CenterAlign(string text, int totalWidth)
	{
		return text
			.PadLeft((totalWidth - text.Length) / 2 + text.Length, ' ')
			.PadRight(totalWidth, ' ');
	}
}
