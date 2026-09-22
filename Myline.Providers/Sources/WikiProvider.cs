using System.Text.RegularExpressions;
using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Core.Utilities.Extensions;
using Myline.Providers.Interfaces;
using Myline.Providers.ResponseModels;

namespace Myline.Providers.Sources;

public class WikiProvider(IList<Uri> apiUrls) : IProvider
{
	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		foreach (var apiUrl in apiUrls)
		{
			var urlParams = new Dictionary<string, string>
			{
				{ "action", "query" },
				{ "list", "usercontribs" },
				{ "ucuser", input.Username.Value },
				{ "ucstart", input.DateRange.To.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
				{ "ucend", input.DateRange.From.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
				{ "ucdir", "older" },
				{ "uclimit", "500" },
				{ "ucprop", "ids|title|timestamp|comment|size|sizediff|flags" },
				{ "format", "json" },
			};
			var result = await WebRequests.Get<WikiApiResponse>(apiUrl, urlParams);
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(result.Error);
			}
			foreach (var edit in result.Value?.Query.UserContributions ?? [])
			{
				var (section, summary) = ParseEditSummary(edit.Comment);
				var diffAmt = edit.SizeDiff < 0 ? edit.SizeDiff.ToString() : "+" + edit.SizeDiff;
				var historyItem = new HistoryItem
				{
					Timestamp = new Timestamp(edit.Timestamp, TimestampPrecision.Second),
					Type = HistoryType.Edit,
					Site = apiUrl.Host,
					Context = edit.Title + (section != null ? " § " + section : ""),
					Description = $"{summary} ({diffAmt})",
				};
				history.Add(historyItem);
			}
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private static (string? Section, string Summary) ParseEditSummary(string summary)
	{
		const string noSummary = "(no summary)";
		const string inernalLinkRegex = @"\[\[(?:.+?\|)?(.+?)\]\]";
		const string sectionSummaryRegex = @"^/\*\s*(.+?)\s*\*/\s*";

		var section = Regex.Match(summary, sectionSummaryRegex).Groups[1].Value;

		summary = summary
			// Substitute link text
			.RegexReplace(inernalLinkRegex, "$1")
			// Better section summaries
			.RegexReplace(sectionSummaryRegex, "");

		if (string.IsNullOrWhiteSpace(section))
		{
			section = null;
		}
		if (string.IsNullOrWhiteSpace(summary))
		{
			summary = noSummary;
		}

		return (section, summary);
	}
}
