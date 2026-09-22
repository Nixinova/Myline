using System.Text.RegularExpressions;
using Myline.Display.ViewModels;

namespace Myline.Display;

public static class CliDisplay
{
	public static void DisplayData(IReadOnlyCollection<ViewHistoryItem> items)
	{
		foreach (var item in items.OrderBy(x => x.Sortkey))
		{
			var line =
				"[" + item.FormattedTimestamp + "] "
				+ item.Site.PadRight(20, ' ')
				+ (item.Type + ": ").PadRight(10, ' ')
				+ item.Context + " - "
				+ item.Description
				;
			line = Regex.Replace(line, @"[\t\r\n]", " ");
			if (line.Length > Console.WindowWidth)
			{
				line = line[..(Console.WindowWidth - 1)] + "…";
			}

			Console.WriteLine(line);
		}
	}
}
