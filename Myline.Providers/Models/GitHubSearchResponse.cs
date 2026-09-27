using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record GitHubSearchResponse
{
	[JsonPropertyName("items")]
	public required IReadOnlyList<GitHubSearchItemResponse> Items { get; init; }

	// int total_count
	// bool incomplete_results
}

public sealed record GitHubSearchItemResponse
{
	[JsonPropertyName("url")]
	public required string Url { get; init; }

	[JsonPropertyName("repository_url")]
	public required string RepositoryUrl { get; init; }

	[JsonPropertyName("html_url")]
	public required string HtmlUrl { get; init; }
	public string Repo => string.Join("/", HtmlUrl.Split("/")[3..5]);

	[JsonPropertyName("node_id")]
	public required string NodeId { get; init; }

	[JsonPropertyName("number")]
	public required int Number { get; init; }

	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("body")]
	public required string Body { get; init; }

	[JsonPropertyName("created_at")]
	public required DateTime CreatedAt { get; init; }

	[JsonPropertyName("updated_at")]
	public required DateTime UpdatedAt { get; init; }

	[JsonPropertyName("closed_at")]
	public required DateTime? ClosedAt { get; init; }

	[JsonPropertyName("author_association")]
	public required string AuthorAssociation { get; init; }

	// id
	// user
	// labels
	// state
	// locked
	// assignee
	// assignees
	// pull_request
	// labels_url
	// events_url
	// comments_url
}
