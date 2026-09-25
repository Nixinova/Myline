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

		var gamesEntries = new List<HltbGameEntry>();
		foreach (var userId in Config.UserIds)
		{
			var result = await GetGamesListForUser(userId);
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(result.Error);
			}
			gamesEntries.AddRange(result.Value.Data.GamesList);
		}

		var gamesList = gamesEntries
			.Where(x => IsEntryWithinDateRange(x, input.DateRange));

		foreach (var entry in gamesList)
		{
			if (input.DateRange.Contains(entry.DateAdded) &&
			    DateOnly.FromDateTime(entry.DateAdded) != entry.DateCompleted)
			{
				var playedOn = entry.Platform + entry.Storefront.IfNotEmpty(x => $" ({x})");
				history.Add(new HistoryItem
				{
					Timestamp = new Timestamp(entry.DateAdded, TimestampPrecision.Second),
					Site = "HowLongToBeat",
					Description = $"Logged \01{entry.GameName}\00 - {playedOn}"
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
					Description = $"{GetVerb(entry)} {Fmt.Prim(entry.GameName)} - {Fmt.Usr(GetDesc(entry))}"
				});
			}
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private static async Task<Result<HowLongToBeatGamesListResponse>> GetGamesListForUser(int userId)
	{
		var apiUrl = new Uri($"https://howlongtobeat.com/api/user/{userId}/games/list");
		var body = new HowLongToBeatGamesListQuery
		{
			UserId = userId,
			ToggleType = HltbQueryToggleType.MultiList,
			Lists = [HltbQueryListType.Completed, HltbQueryListType.Replayed, HltbQueryListType.Retired],
			Limit = 500,
			CurrentUserHome = true
		};
		var result = await WebRequests.Post<HowLongToBeatGamesListQuery, HowLongToBeatGamesListResponse>(apiUrl, body);
		if (result.IsError)
		{
			return Result<HowLongToBeatGamesListResponse>.Fail(result.Error);
		}
		return Result<HowLongToBeatGamesListResponse>.Ok(result.Value!);
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
		var notes = string.Join(" | ", new[]
		{
			entry.PlayNotes,
			string.IsNullOrWhiteSpace(entry.ReviewNotes) && entry.ReviewScore == 0 ? "" : $"{entry.ReviewScore / 10}/10 {entry.ReviewNotes}".Trim(),
			entry.CompMainNotes,
			entry.CompMainPlusNotes,
			entry.Comp100Notes,
			entry.CompSpeedNotes,
			entry.CompSpeed100Notes,
		}.Where(x => !string.IsNullOrWhiteSpace(x)));
		return string.Join(" ", new[]
		{
			entry.Platform,
			entry.Storefront.IfNotEmpty(x => $"({x})"),
			entry.DateCompleted == null || entry.CompletionTimeMain == null ? "" : "- " + entry.CompletionTimeMain.ToString(),
			notes.IfNotEmpty(x => $"| {x}")
		}.Where(x => !string.IsNullOrWhiteSpace(x)));
	}
}
