using System.Text.Json.Serialization;

namespace Myline.Providers.ResponseModels;

public record GitHubContributionsQuery
{
	[JsonPropertyName("query")]
	public required string Query { get; init; }

	[JsonPropertyName("variables")]
	public required dynamic Variables { get; init; }
}
