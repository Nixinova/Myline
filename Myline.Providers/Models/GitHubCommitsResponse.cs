using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public record GitHubCommitsResponse
{
	[JsonPropertyName("commit")]
	public required GitHubCommitResponse Commit { get; init; }

	[JsonPropertyName("author")]
	public required GitHubUserResponse Author { get; init; }

	[JsonPropertyName("committer")]
	public required GitHubUserResponse Committer { get; init; }

	[JsonPropertyName("url")]
	public required string Url { get; init; }

	public string RepoOwner => Url.Split('/')[4];

	public string RepoName => Url.Split('/')[5];

	// sha
	// node_id
	// html_url
	// comments_url
	// parents
}

public record GitHubCommitResponse
{
	[JsonPropertyName("author")]
	public required GitHubCommitAuthorResponse Authored { get; init; }

	[JsonPropertyName("committer")]
	public required GitHubCommitAuthorResponse Committed { get; init; }

	[JsonPropertyName("message")]
	public required string Message { get; init; }

	// tree
	// url
	// comment_count
	// verification
}

public record GitHubUserResponse
{
	[JsonPropertyName("login")]
	public required string Name { get; init; }

	[JsonPropertyName("type")]
	public required string Type { get; init; }

	// id
	// node_id
	// gravatar_id
	// avatar_url
	// url
	// html_url
	// followers_url
	// following_url
	// gists_url
	// starred_url
	// subscriptions_url
	// organizations_url
	// repos_url
	// events_url
	// received_events_url
	// user_view_type
	// site_admin
}

public record GitHubCommitAuthorResponse
{
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	[JsonPropertyName("email")]
	public required string Email { get; init; }

	[JsonPropertyName("date")]
	public required DateTime Date { get; init; }
}
