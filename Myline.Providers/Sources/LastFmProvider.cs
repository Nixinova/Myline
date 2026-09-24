using System.Globalization;
using Myline.Core.Configuration;
using Myline.Core.Configuration.Models;
using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Core.Utilities.Extensions;
using Myline.Providers.Interfaces;
using Myline.Providers.Models;

namespace Myline.Providers.Sources;

public class LastFmProvider : IProvider
{
	private static LastFmConfig Config => ConfigStore.Config.LastFmConfig;
	private static readonly Uri ApiUrl = new("https://ws.audioscrobbler.com/2.0/");
	private const string ApiKey = "29928f386eb7f4f024598d42628b1428";

	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		var responses = new List<LastFmRecentTracksResponse>();
		foreach(var username in Config.Usernames)
		{
			var result = await GetPlaysForUser(input, username);
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(result.Error);
			}
			responses.Add(result.Value);
		}

		foreach (var track in responses.SelectMany(x => x.RecentTracks.Tracks))
		{
			if (track.Date is null) continue; // Skip 'Now playing'
			var uts = long.Parse(track.Date.UnixTimeSeconds);
			var date = DateTimeOffset.FromUnixTimeSeconds(uts).DateTime;
			var historyItem = new HistoryItem
			{
				Timestamp = new Timestamp(date, TimestampPrecision.Second),
				Site = "Last.fm",
				Description = $"Listened to {track.Song} by {track.Artist.Value}" +
				              track.Album.Value.IfNotEmpty(x => $" from {x}")
			};
			history.Add(historyItem);
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private async Task<Result<LastFmRecentTracksResponse>> GetPlaysForUser(ProviderInput input, string username)
	{
		var fromDateUts = new DateTimeOffset(input.DateRange.From).ToUnixTimeSeconds();
		var toDateUts = new DateTimeOffset(input.DateRange.To).ToUnixTimeSeconds();
		var urlParams = new Dictionary<string, string>
		{
			{ "method", "user.getRecentTracks" },
			{ "user", username },
			{ "api_key", ApiKey },
			{ "from", fromDateUts.ToString(CultureInfo.InvariantCulture) },
			{ "to", toDateUts.ToString(CultureInfo.InvariantCulture) },
			{ "limit", "200" },
			{ "format", "json" },
		};
		var result = await WebRequests.Get<LastFmRecentTracksResponse>(ApiUrl, urlParams);
		if (result.IsError)
		{
			return Result<LastFmRecentTracksResponse>.Fail(result.Error);
		}
		return Result<LastFmRecentTracksResponse>.Ok(result.Value!);
	}
}
