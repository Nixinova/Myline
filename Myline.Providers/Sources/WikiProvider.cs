using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Providers.Interfaces;
using Myline.Providers.ResponseModels;

namespace Myline.Providers.Sources;

public class WikiProvider : IProvider
{
	private readonly IList<Uri> _wikiApiUrls = [];

	public WikiProvider(IList<Uri> wikiApiUrls)
	{
		foreach (var wikiApiUrl in wikiApiUrls)
		{
			_wikiApiUrls.Add(wikiApiUrl);
		}
	}

	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		foreach (var apiUrl in _wikiApiUrls)
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
			var responseObj = await WebRequests.Get<WikiApiResponse>(apiUrl, urlParams);
			foreach (var edit in responseObj?.Query.UserContributions ?? [])
			{
				var editSummary = string.IsNullOrWhiteSpace(edit.Comment) ? "(no summary)" : edit.Comment;
				var diffAmt = edit.SizeDiff < 0 ? edit.SizeDiff.ToString() : "+" + edit.SizeDiff;
				var historyItem = new HistoryItem
				{
					Timestamp = new Timestamp(edit.Timestamp, TimestampPrecision.Second),
					Type = HistoryType.Edit,
					Site = apiUrl.Host,
					Context = edit.Title,
					Description = $"{editSummary} ({diffAmt})",
				};
				history.Add(historyItem);
			}
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}
}
