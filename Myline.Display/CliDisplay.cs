using Myline.Display.ViewModels;

namespace Myline.Display;

public static class CliDisplay
{
	public static void DisplayData(IReadOnlyCollection<ViewHistoryItem> items)
	{
		foreach (var item in items.OrderBy(x => x.Sortkey))
		{
			Console.WriteLine(
				"[" + item.FormattedTimestamp + "] "
				+ item.Site.PadRight(20, ' ')
				+ (item.Type + ": ").PadRight(10, ' ')
				+ item.Context + " - "
				+ item.Description
			);
		}
	}
}
