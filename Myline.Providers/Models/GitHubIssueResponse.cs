using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record GitHubIssueResponse
{
	[JsonPropertyName("number")]
	public required int Number { get; init; }

	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("body")]
	public required string Body { get; init; }

	[JsonPropertyName("html_url")]
	public required string Url { get; init; }
	public string Repo => string.Join("/", Url.Split("/")[3..5]);

	[JsonPropertyName("created_at")]
	public required DateTime CreatedAt { get; init; }

	[JsonPropertyName("updated_at")]
	public DateTime UpdatedAt { get; init; }

	[JsonPropertyName("closed_at")]
	public DateTime? ClosedAt { get; init; }

	// state
	// locked
	// comments
	// labels
	// assignee
	// assignees
}

public sealed record GitHubIssueUserResponse
{
	[JsonPropertyName("id")]
	public required int Id { get; init; }

	[JsonPropertyName("login")]
	public required string Username { get; init; }
}
