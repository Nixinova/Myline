using Myline.Core.Utilities;
using Myline.Core.Utilities.Extensions;
using Myline.Display.ViewModels;

namespace Myline.Display;

public static class CliDisplay
{
	private const string AnsiReset = "\e[0m";

	public static void DisplayData(IReadOnlyCollection<ViewHistoryItem> items)
	{
		var maxTimestampLen = items.Select(x => x.FormattedTimestamp.Length).SafeMax();
		var maxSiteLen = items.Select(x => x.Site.Length).SafeMax();
		foreach (var item in items.OrderBy(x => x.Sortkey))
		{
			var line = string.Join(" ",
				item.FormattedTimestamp.PadRight(maxTimestampLen, ' ') + " ",
				AutoColour(CenterAlign(item.Site, maxSiteLen)) + " ",
				item.Description
			);

			// Convert escapes to ansi formatting codes
			line = line
				.Replace(OutputFormatHelper.Reset, AnsiReset)
				.Replace(OutputFormatHelper.Primary, "\e[1;38;5;231m")
				.Replace(OutputFormatHelper.Secondary, "\e[0;38;5;231m")
				.Replace(OutputFormatHelper.Tertiary, "\e[0;3;38;5;231m")
				.Replace(OutputFormatHelper.Addendum, "\e[3;37m");

			// Construct output line, truncated to console window width, without mistruncating the ansi codes
			var outputLine = "";
			var visibleIndex = 0;
			for (var i = 0; i < line.Length; i++)
			{
				if (line[i] == '\e')
				{
					while (line[i] != 'm')
					{
						outputLine += line[i];
						i++;
					}
				}
				else
				{
					visibleIndex++;
				}
				outputLine += line[i];
				if (visibleIndex == Console.WindowWidth - 1)
				{
					outputLine += "…";
					break;
				}
			}
			outputLine += AnsiReset;

			Console.WriteLine(outputLine);
		}
	}

	private static string CenterAlign(string text, int totalWidth)
	{
		return text
			.PadLeft((totalWidth - text.Length) / 2 + text.Length, ' ')
			.PadRight(totalWidth, ' ');
	}

	private static string AutoColour(string str)
	{
		var hash = str.Trim().ToCharArray().Aggregate(0,
			(cur, ch) => (cur * 32 + ch) & 0xFFFFFF
		);
		const byte minChanVal = 0x60;
		const byte maxChanVal = 0xFF;
		var r = minChanVal + ((hash >> 16) & maxChanVal) * (maxChanVal - minChanVal) / maxChanVal;
		var g = minChanVal + ((hash >> 8) & maxChanVal) * (maxChanVal - minChanVal) / maxChanVal;
		var b = minChanVal + (hash & maxChanVal) * (maxChanVal - minChanVal) / maxChanVal;

		var ansi = "\e[1;38;2;" + r + ";" + g + ";" + b + "m";
		return ansi + str + AnsiReset;
	}
}
