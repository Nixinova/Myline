using Myline.Display.ViewModels;

namespace Myline.Display;

public static class CliDisplay
{
	public static void DisplayData(IReadOnlyCollection<ViewHistoryItem> items)
	{
		foreach (var item in items)
		{
			Console.WriteLine($"[{item.FormattedTimestamp}] {item.Type}: {item.Title}");
		}
	}
}
