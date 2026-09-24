using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public record LastFmRecentTracksResponse
{
	[JsonPropertyName("recenttracks")]
	public required LastFmRecentTracks RecentTracks { get; init; }
}

public record LastFmRecentTracks
{
	[JsonPropertyName("@attr")]
	public required object Attributes { get; init; }

	[JsonPropertyName("track")]
	public required IList<LastFmTrack> Tracks { get; init; }
}

public record LastFmTrack
{
	/// Null if now playing
	[JsonPropertyName("date")]
	public LastFmDate? Date { get; init; }

	[JsonPropertyName("url")]
	public required string Url { get; init; }

	[JsonPropertyName("artist")]
	public required LastFmTrackDataItem Artist { get; init; }

	[JsonPropertyName("album")]
	public required LastFmTrackDataItem Album { get; init; }

	[JsonPropertyName("name")]
	public required string Song { get; init; }

	[JsonPropertyName("image")]
	public required IList<object> AlbumImage { get; init; }

	/// Empty string when not present
	[JsonPropertyName("mbid")]
	public string MbGuid { get; init; } = "";

	[JsonPropertyName("streamable")]
	public required string Streamable { get; init; }
}

public record LastFmTrackDataItem
{
	/// Empty string when not present
	[JsonPropertyName("mbid")]
	public string MbGuid { get; init; } = "";

	[JsonPropertyName("#text")]
	public required string Value { get; init; }
}

public record LastFmDate
{
	/// Empty string when not present
	[JsonPropertyName("uts")]
	public required string UnixTimeSeconds { get; init; }

	[JsonPropertyName("#text")]
	public required string Display { get; init; }
}
