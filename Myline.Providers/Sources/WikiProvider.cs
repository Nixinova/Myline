using System.Text.RegularExpressions;
using Myline.Core.Configuration;
using Myline.Core.Configuration.Models;
using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Core.Utilities.Extensions;
using Myline.Providers.Interfaces;
using Myline.Providers.Models;

namespace Myline.Providers.Sources;

public class WikiProvider : IProvider
{
	private static WikiConfig Config => ConfigStore.Config.WikiConfig;

	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		foreach (var apiUrl in Config.ApiUrls)
		{
			var apiUri = new Uri(apiUrl);
			var username = Config.UsernamesPerWikiDomain[apiUri.Host];

			var contribsResult = await GetContributions(apiUri, input, username);
			if (contribsResult.IsError)
			{
				Console.WriteLine($"Error ({apiUri.Host}): {contribsResult.Error}");
				continue;
			}
			var logsResult = await GetLogs(apiUri, input, username);
			if (logsResult.IsError)
			{
				Console.WriteLine($"Error ({apiUri.Host}): {logsResult.Error}");
				continue;
			}
			List<IWikiEvent> wikiEvents = [.. contribsResult.Value, .. logsResult.Value];
			foreach (var edit in wikiEvents)
			{
				if (edit is WikiLogEvent { Type: "create" or "upload" })
				{
					// These double as edits
					continue;
				}

				var (section, summary) = ParseEditSummary(edit);
				var historyItem = new HistoryItem
				{
					Timestamp = new Timestamp(edit.Timestamp, TimestampPrecision.Second),
					Site = apiUri.Host,
					Action = edit switch
					{
						WikiContribution => "Edited",
						WikiLogEvent le => MapAction(le.Action),
						_ => "Affected"
					},
					Context = edit.Title + (section != null ? " § " + section : ""),
					Description = summary
				};
				history.Add(historyItem);
			}
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private static async Task<Result<IReadOnlyList<IWikiEvent>>> GetContributions(Uri apiUri, ProviderInput input, string username)
	{
		var urlParams = new Dictionary<string, string>
		{
			{ "action", "query" },
			{ "list", "usercontribs" },
			{ "ucuser", username },
			{ "ucstart", input.DateRange.To.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
			{ "ucend", input.DateRange.From.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
			{ "ucdir", "older" },
			{ "uclimit", "500" },
			{ "ucprop", "ids|title|timestamp|comment|size|sizediff|flags" },
			{ "format", "json" },
		};
		var result = await WebRequests.Get<WikiApiResponse>(apiUri, urlParams);
		if (result.IsError)
		{
			return Result<IReadOnlyList<IWikiEvent>>.Fail(result.Error);
		}
		return Result<IReadOnlyList<IWikiEvent>>.Ok(result.Value!.Query.UserContributions!);
	}

	private static async Task<Result<IReadOnlyList<IWikiEvent>>> GetLogs(Uri apiUri, ProviderInput input, string username)
	{
		var urlParams = new Dictionary<string, string>
		{
			{ "action", "query" },
			{ "list", "logevents" },
			{ "leuser", username },
			{ "lestart", input.DateRange.To.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
			{ "leend", input.DateRange.From.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") },
			{ "ledir", "older" },
			{ "lelimit", "500" },
			{ "leprop", "ids|title|timestamp|comment|type|action|user|details" },
			{ "format", "json" },
		};
		var result = await WebRequests.Get<WikiApiResponse>(apiUri, urlParams);
		if (result.IsError)
		{
			return Result<IReadOnlyList<IWikiEvent>>.Fail(result.Error);
		}
		return Result<IReadOnlyList<IWikiEvent>>.Ok(result.Value!.Query.LogEvents!);
	}

	private static (string? Section, string Summary) ParseEditSummary(IWikiEvent item)
	{
		const string noSummary = "(no summary)";
		const string inernalLinkRegex = @"\[\[(?:.+?\|)?(.+?)\]\]";
		const string sectionSummaryRegex = @"^/\*\s*(.+?)\s*\*/\s*";

		var summary = item.Comment;
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
		if (item is WikiContribution wc)
		{
			var diff = wc.SizeDiff < 0 ? wc.SizeDiff.ToString() : "+" + wc.SizeDiff;
			summary += $" ({diff})";
		}

		return (section, summary);
	}

	private static string MapAction(string action)
	{
		return action switch
		{
			"upload" => "Uploaded",
			"delete" => "Deleted",
			"patrol" => "Patrolled",
			"thank" => "Thanked",
			_ => action
		};
	}
}
