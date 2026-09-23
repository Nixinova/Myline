using Myline.Core.Configuration;
using Myline.Core.Configuration.Models;
using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Core.Utilities.Extensions;
using Myline.Providers.Interfaces;
using Myline.Providers.Models;

namespace Myline.Providers.Sources;

public class HowLongToBeatProvider : IProvider
{
	private static HltbConfig Config => ConfigStore.Config.HltbConfig;

	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		var apiUrl = new Uri($"https://howlongtobeat.com/api/user/{Config.UserId}/games/list");
		var body = new HowLongToBeatGamesListQuery
		{
			UserId = Config.UserId,
			ToggleType = HltbQueryToggleType.MultiList,
			Lists = [HltbQueryListType.Completed, HltbQueryListType.Replayed, HltbQueryListType.Retired],
			Limit = 500,
			CurrentUserHome = true
		};
		var result = await WebRequests.Post<HowLongToBeatGamesListQuery, HowLongToBeatGamesListResponse>(apiUrl, body);
		if (result.IsError)
		{
			return Result<IReadOnlyCollection<HistoryItem>>.Fail(result.Error);
		}

		var gamesList = result.Value!.Data.GamesList
			.Where(x => IsEntryWithinDateRange(x, input.DateRange));

		foreach (var entry in gamesList)
		{
			if (input.DateRange.Contains(entry.DateAdded))
			{
				history.Add(new HistoryItem
				{
					Timestamp = new Timestamp(entry.DateAdded, TimestampPrecision.Second),
					Site = "HowLongToBeat",
					Action = "Logged",
					Context = entry.GameName,
					Description = GetDesc(entry)
				});
			}
			if (entry.DateCompleted != null && input.DateRange.Contains(entry.DateCompleted.Value))
			{
				history.Add(new HistoryItem
				{
					Timestamp = DateOnly.FromDateTime(entry.DateUpdated.Date) == entry.DateCompleted.Value
						? new Timestamp(entry.DateUpdated, TimestampPrecision.Second)
						: new Timestamp(entry.DateCompleted.Value, TimestampPrecision.Day),
					Site = "HowLongToBeat",
					Action = GetVerb(entry),
					Context = entry.GameName,
					Description = GetDesc(entry)
				});
			}
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private static bool IsEntryWithinDateRange(HltbGameEntry entry, DateRange dateRange)
	{
		return dateRange.Contains(entry.DateAdded) ||
		       dateRange.Contains(entry.DateUpdated) ||
		       entry.DateCompleted != null && dateRange.Contains(entry.DateCompleted.Value);
	}

	private static string GetVerb(HltbGameEntry entry)
	{
		string? verb = null;
		if (entry.InListCompleted) verb = "Completed";
		if (entry.InListPlaying) verb = "Playing";
		if (entry.InListBacklog) verb = "Backlogged";
		if (entry.InListReplay) verb = "Replayed";
		if (entry.InListRetired) verb = "Retired";
		return verb ?? "Played";
	}

	private static string GetDesc(HltbGameEntry entry)
	{
		return string.Join(" ", [
			entry.Platform,
			string.IsNullOrWhiteSpace(entry.Storefront) ? "" : $"({entry.Storefront})",
			string.IsNullOrWhiteSpace(entry.PlayNotes) ? "" : "- " + entry.PlayNotes,
			string.IsNullOrWhiteSpace(entry.ReviewNotes) ? "" : "- " + entry.ReviewNotes,
		]).RegexReplace(@"\s+", " ");
	}
}
