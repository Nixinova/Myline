using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record HowLongToBeatGamesListResponse
{
	[JsonPropertyName("data")]
	public required HltbGamesData Data { get; init; }
}

public sealed record HltbGamesData
{
	[JsonPropertyName("count")]
	public required int Count { get; init; }

	[JsonPropertyName("total")]
	public required int Total { get; init; }

	[JsonPropertyName("gamesList")]
	public required HltbGameEntry[] GamesList { get; init; }

	[JsonPropertyName("platformList")]
	public required object[] PlatformList { get; init; }

	[JsonPropertyName("summaryData")]
	public required object SummaryData { get; init; }
}

public sealed record HltbGameEntry
{
	[JsonPropertyName("id")]
	public required int Id { get; init; }

	[JsonPropertyName("custom_title")]
	public required string GameName { get; init; }

	[JsonPropertyName("platform")]
	public required string Platform { get; init; }

	[JsonPropertyName("play_storefront")]
	public required string Storefront { get; init; }

	/// Percentage review score
	[JsonPropertyName("review_score")]
	public required byte ReviewScore { get; init; }

	[JsonPropertyName("review_notes")]
	public required string ReviewNotes { get; init; }

	[JsonPropertyName("play_notes")]
	public required string PlayNotes { get; init; }

	/// YYYY-MM-DD fallback 0000-00-00
	[JsonPropertyName("date_complete")]
	public required string DateCompleteRaw { private get; init; }
	public DateOnly? DateCompleted => DateCompleteRaw == "0000-00-00" ? null : DateOnly.Parse(DateCompleteRaw);

	/// YYYY-MM-DD hh:mm:ss
	[JsonPropertyName("date_updated")]
	public required string DateUpdatedRaw { private get; init; }
	public DateTime DateUpdated => DateTime.Parse(DateUpdatedRaw);

	/// YYYY-MM-DD hh:mm:ss
	[JsonPropertyName("date_added")]
	public required string DateAddedRaw { private get; init; }
	public DateTime DateAdded => DateTime.Parse(DateAddedRaw);

	[JsonPropertyName("list_playing")]
	public required int ListPlayingRaw { private get; init; }
	public bool InListPlaying => ListPlayingRaw == 1;

	[JsonPropertyName("list_backlog")]
	public required int ListBacklogRaw { private get; init; }
	public bool InListBacklog => ListBacklogRaw == 1;

	[JsonPropertyName("list_replay")]
	public required int ListReplayRaw { private get; init; }
	public bool InListReplay => ListReplayRaw == 1;

	[JsonPropertyName("list_comp")]
	public required int ListCompletedRaw { private get; init; }
	public bool InListCompleted => ListCompletedRaw == 1;

	[JsonPropertyName("list_retired")]
	public required int ListRetiredRaw { private get; init; }
	public bool InListRetired => ListRetiredRaw == 1;

	[JsonPropertyName("comp_main")]
	public required int CompMainSecs { private get; init; }

	public TimeSpan? CompletionTimeMain => CompMainSecs == 0 ? null : TimeSpan.FromSeconds(CompMainSecs);

	[JsonPropertyName("comp_plus")]
	public required int CompPlusSecs { private get; init; }
	public TimeSpan? CompletionTimeMainPlus => CompPlusSecs == 0 ? null : TimeSpan.FromSeconds(CompPlusSecs);

	[JsonPropertyName("comp_100")]
	public required int Comp100Secs { private get; init; }
	public TimeSpan? CompletionTime100 => Comp100Secs == 0 ? null : TimeSpan.FromSeconds(Comp100Secs);

	[JsonPropertyName("comp_speed")]
	public required int CompSpeedSecs { private get; init; }
	public TimeSpan? CompletionTimeSpeedrun => CompSpeedSecs == 0 ? null : TimeSpan.FromSeconds(CompSpeedSecs);

	[JsonPropertyName("comp_speed100")]
	public required int CompSpeed100Secs { private get; init; }
	public TimeSpan? CompletionTimeSpeedrun100 => CompSpeed100Secs == 0 ? null : TimeSpan.FromSeconds(CompSpeed100Secs);

	[JsonPropertyName("comp_main_notes")]
	public required string CompMainNotes { get; init; }

	[JsonPropertyName("comp_plus_notes")]
	public required string CompMainPlusNotes { get; init; }

	[JsonPropertyName("comp_100_notes")]
	public required string Comp100Notes { get; init; }

	[JsonPropertyName("comp_speed_notes")]
	public required string CompSpeedNotes { get; init; }

	[JsonPropertyName("comp_speed100_notes")]
	public required string CompSpeed100Notes { get; init; }


	// int list_custom
	// int list_custom2
	// int list_custom3
	// int invested_pro
	// int invested_sp
	// int invested_spd
	// int invested_co
	// int invested_mp
	// int play_count
	// int play_dlc
	// string? retired_notes
	// DateOnly date_start // YYYY-MM-DD fallback 0000-00-00
	// string play_video
	// int game_id
	// string game_image // filename
	// string game_type // enum
	// DateOnly release_world
	// int comp_all
	// int comp_main_g
	// int review_score_g
}
