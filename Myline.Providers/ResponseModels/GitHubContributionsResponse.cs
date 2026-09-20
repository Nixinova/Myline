using System.Text.Json.Serialization;

namespace Myline.Providers.ResponseModels;

public record GitHubContributionsResponse
{
	[JsonPropertyName("data")]
	public required GitHubContributionsData Data { get; init; }
}

public record GitHubContributionsData
{
	[JsonPropertyName("user")]
	public required GitHubUserContributionData UerData { get; init; }
}

public record GitHubUserContributionData
{
	[JsonPropertyName("contributionsCollection")]
	public required GitHubContributionsCollection Contributions { get; init; }
}

public record GitHubContributionsCollection
{
	[JsonPropertyName("commitContributionsByRepository")]
	public required IList<GitHubRepoContributionsData> Data { get; init; }
}

public record GitHubRepoContributionsData
{
	[JsonPropertyName("repository")]
	public required IList<ContributionRepoData> Data { get; init; }
}

public record ContributionRepoData
{
	[JsonPropertyName("owner")]
	public required RepoOwner Owner { get; init; }
	[JsonPropertyName("name")]
	public required string Name { get; init; }
	[JsonPropertyName("nameWithOwner")]
	public required string FullName { get; init; }
}

public record RepoOwner
{
	[JsonPropertyName("login")]
	public required string Name { get; init; }
}
