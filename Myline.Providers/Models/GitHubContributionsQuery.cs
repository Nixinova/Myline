using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record GitHubContributionsQuery
{
	[JsonPropertyName("query")]
	public required string Query { get; init; }

	[JsonPropertyName("variables")]
	public required dynamic Variables { get; init; }
}
