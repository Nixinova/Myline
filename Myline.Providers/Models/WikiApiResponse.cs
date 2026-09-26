using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record WikiApiResponse
{
	[JsonPropertyName("batchcomplete")]
	public string BatchComplete { get; init; } = "";

	[JsonPropertyName("query")]
	public WikiQueryResponse Query { get; init; } = new();
}

public sealed record WikiQueryResponse
{
	[JsonPropertyName("usercontribs")]
	public WikiContribution[]? UserContributions { get; init; } = [];

	[JsonPropertyName("logevents")]
	public WikiLogEvent[]? LogEvents { get; init; } = [];
}

public interface IWikiEvent
{
	public string Title { get; }
	public DateTime Timestamp { get; }
	public string? Comment { get; }
}

public sealed record WikiContribution : IWikiEvent
{
	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("timestamp")]
	public required DateTime Timestamp { get; init; }

	[JsonPropertyName("comment")]
	public string? Comment { get; init; }

	[JsonPropertyName("sizediff")]
	public required long SizeDiff { get; init; }

	// int userid
	// string user
	// int pageid
	// int revid
	// int parentid
	// int ns
	// string? top
	// int size
}

public sealed record WikiLogEvent : IWikiEvent
{
	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("timestamp")]
	public required DateTime Timestamp { get; init; }

	[JsonPropertyName("comment")]
	public string? Comment { get; init; }

	[JsonPropertyName("type")]
	public required string Type { get; init; }

	[JsonPropertyName("action")]
	public required string Action { get; init; }

	// int logid
	// int ns
	// string user
	// int pageid
	// int logpage
	// object params
}
