using System.Text.Json.Serialization;

namespace Myline.Providers.Models;

public sealed record GitHubPrResponse
{
	[JsonPropertyName("url")]
	public required string Url { get; init; }

	[JsonPropertyName("number")]
	public required int Number { get; init; }

	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("body")]
	public required string Body { get; init; }

	[JsonPropertyName("created_at")]
	public required DateTime CreatedAt { get; init; }

	[JsonPropertyName("updated_at")]
	public DateTime? UpdatedAt { get; init; }

	[JsonPropertyName("closed_at")]
	public DateTime? ClosedAt { get; init; }

	[JsonPropertyName("merged_at")]
	public DateTime? MergedAt { get; init; }

	[JsonPropertyName("user")]
	public required GitHubPrUserResponse User { get; init; }

	[JsonPropertyName("head")]
	public required GitHubPrBaseResponse Head { get; init; }

	[JsonPropertyName("base")]
	public required GitHubPrBaseResponse Base { get; init; }

	// state: string
	// locked: bool
	// merged: bool
	// mergeable: bool
	// draft: bool
	// commits: int
	// additions: int
	// deletions: int
	// changed_files: int
	// html_url: string
}

public sealed record GitHubPrUserResponse
{
	[JsonPropertyName("id")]
	public required int Id { get; init; }

	[JsonPropertyName("login")]
	public required string Username { get; init; }
}

public sealed record GitHubPrBaseResponse
{
	[JsonPropertyName("label")]
	public required string Label { get; init; }

	[JsonPropertyName("ref")]
	public required string Ref { get; init; }

	[JsonPropertyName("sha")]
	public required string Sha { get; init; }

	[JsonPropertyName("repo")]
	public required GitHubPrRepoResponse Repo { private get; init; }
	public string RepoName => Repo.Owner + "/" + Repo.Name;
}

public sealed record GitHubPrRepoResponse
{
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	[JsonPropertyName("full_name")]
	public required string Owner { get; init; }
}
