using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public record HowLongToBeatGamesListQuery
{
	[JsonPropertyName("user_id")]
	public required int UserId { get; init; }

	[JsonPropertyName("toggleType")]
	public required HltbQueryToggleType ToggleType { get; init; }

	[JsonPropertyName("lists")]
	public required HltbQueryListType[] Lists { get; init; } = [];

	[JsonPropertyName("set_playstyle")]
	public HltbQueryPlayStyle SetPlaystyle { get; init; } = HltbQueryPlayStyle.CompMain;

	[JsonPropertyName("name")]
	public string Name { get; init; } = "";

	[JsonPropertyName("platform")]
	public HowLongToBeatGamesListFilter Platform { get; init; } = new();

	[JsonPropertyName("storefront")]
	public HowLongToBeatGamesListFilter Storefront { get; init; } = new();

	[JsonPropertyName("sortBy")]
	public string SortBy { get; init; } = "";

	[JsonPropertyName("sortFlip")]
	public int SortFlip { get; init; } = 0;

	[JsonPropertyName("view")]
	public string View { get; init; } = "";

	[JsonPropertyName("random")]
	public int Random { get; init; } = 0;

	[JsonPropertyName("limit")]
	public required int Limit { get; init; }

	[JsonPropertyName("currentUserHome")]
	public required bool CurrentUserHome { get; init; }
}

public record HowLongToBeatGamesListFilter
{
	[JsonPropertyName("mode")]
	public HltbQueryFilterMode Mode { get; init; } = HltbQueryFilterMode.Include;

	[JsonPropertyName("values")]
	public object[] Values { get; init; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HltbQueryToggleType
{
	[JsonStringEnumMemberName("Multi List")]
	MultiList,
	[JsonStringEnumMemberName("Single List")]
	SingleList,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HltbQueryListType
{
	[JsonStringEnumMemberName("completed")]
	Completed,
	[JsonStringEnumMemberName("retired")]
	Retired,
	[JsonStringEnumMemberName("replays")]
	Replayed,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HltbQueryPlayStyle
{
	[JsonStringEnumMemberName("comp_main")]
	CompMain,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HltbQueryFilterMode
{
	[JsonStringEnumMemberName("include")]
	Include,
}
