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
			var timestamp = GetTimestamp(entry);

			history.Add(new HistoryItem
			{
				Timestamp = timestamp,
				Site = "HowLongToBeat",
				Action = "Played",
				Context = entry.GameName,
				Description = GetDesc(entry)
			});
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private static bool IsEntryWithinDateRange(HltbGameEntry entry, DateRange dateRange)
	{
		return dateRange.Contains(entry.DateAdded) ||
		       dateRange.Contains(entry.DateUpdated) ||
		       entry.DateCompleted != null && dateRange.Contains(entry.DateCompleted.Value);
	}

	private static Timestamp GetTimestamp(HltbGameEntry entry)
	{
		var dateAdded = new Timestamp(entry.DateAdded, TimestampPrecision.Second);
		if (entry.DateCompleted == null ||
		    DateOnly.FromDateTime(entry.DateAdded.Date) == entry.DateCompleted)
		{
			return dateAdded;
		}

		return new Timestamp(
			new DateTime(entry.DateCompleted.Value.Year, entry.DateCompleted.Value.Month, entry.DateCompleted.Value.Day),
			TimestampPrecision.Day
		);
	}

	private static string GetDesc(HltbGameEntry entry)
	{
		var verb = "";
		if (entry.InListCompleted) verb = "Completed";
		if (entry.InListPlaying) verb = "Playing";
		if (entry.InListBacklog) verb = "Backlogged";
		if (entry.InListReplay) verb = "Replayed";
		if (entry.InListRetired) verb = "Retired";
		return string.Join(" ", [
			verb,
			"-",
			entry.Platform,
			string.IsNullOrWhiteSpace(entry.Storefront) ? "" : $"({entry.Storefront})",
			string.IsNullOrWhiteSpace(entry.PlayNotes) ? "" : "- " + entry.PlayNotes,
			string.IsNullOrWhiteSpace(entry.ReviewNotes) ? "" : "- " + entry.ReviewNotes,
		]).RegexReplace(@"\s+", " ");
	}
}
