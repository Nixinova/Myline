using System.Text.Json.Serialization;

namespace Myline.Providers.ResponseModels;

public record WikiApiResponse
{
	[JsonPropertyName("batchcomplete")]
	public string BatchComplete { get; init; } = "";

	[JsonPropertyName("query")]
	public WikiContributionsQuery Query { get; init; } = new();
}

public record WikiContributionsQuery
{
	[JsonPropertyName("usercontribs")]
	public WikiContribution[] UserContributions { get; init; } = [];
}

public record WikiContribution
{
	[JsonPropertyName("userid")]
	public required long UserId { get; init; }

	[JsonPropertyName("user")]
	public required string Username { get; init; }

	[JsonPropertyName("pageid")]
	public required long PageId { get; init; }

	[JsonPropertyName("revid")]
	public required long RevisionId { get; init; }

	[JsonPropertyName("parentid")]
	public required long ParentId { get; init; }

	[JsonPropertyName("ns")]
	public required long NamespaceId { get; init; }

	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("timestamp")]
	public required DateTime Timestamp { get; init; }

	[JsonPropertyName("top")]
	public string? Top { get; init; }

	[JsonPropertyName("comment")]
	public required string Comment { get; init; }

	[JsonPropertyName("size")]
	public required long Size { get; init; }

	[JsonPropertyName("sizediff")]
	public required long SizeDiff { get; init; }
}
