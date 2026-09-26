using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record GitHubContributionsResponse
{
	[JsonPropertyName("data")]
	public required GitHubContributionsData Data { get; init; }
}

public sealed record GitHubContributionsData
{
	[JsonPropertyName("user")]
	public required GitHubUserContributionData UserData { get; init; }
}

public sealed record GitHubUserContributionData
{
	[JsonPropertyName("contributionsCollection")]
	public required GitHubContributionsCollection Contributions { get; init; }
}

public sealed record GitHubContributionsCollection
{
	[JsonPropertyName("commitContributionsByRepository")]
	public required IReadOnlyList<GitHubRepoContributionsData> Commits { get; init; }

	[JsonPropertyName("pullRequestContributionsByRepository")]
	public required IReadOnlyList<GitHubRepoContributionsData> Prs { get; init; }

	[JsonPropertyName("issueContributionsByRepository")]
	public required IReadOnlyList<GitHubRepoContributionsData> Issues { get; init; }
}

public sealed record GitHubRepoContributionsData
{
	[JsonPropertyName("repository")]
	public required ContributionRepoData Data { get; init; }
}

public sealed record ContributionRepoData
{
	[JsonPropertyName("owner")]
	public required RepoOwner Owner { get; init; }

	[JsonPropertyName("name")]
	public required string Name { get; init; }

	[JsonPropertyName("nameWithOwner")]
	public required string FullName { get; init; }
}

public sealed record RepoOwner
{
	[JsonPropertyName("login")]
	public required string Name { get; init; }
}
