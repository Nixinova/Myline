using System.Net.Http.Headers;
using System.Text.Json;
using Myline.Core.Configuration;
using Myline.Core.Configuration.Models;
using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Providers.Interfaces;
using Myline.Providers.Models;

namespace Myline.Providers.Sources;

public class GitHubProvider : IProvider
{
	private static GitHubConfig Config => ConfigStore.Config.GitHubConfig;
	private static readonly Uri GitHubGraphQlUrl = new("https://api.github.com/graphql");
	private const string ContributionsGraphQl =
		"""
		query UserCommitRepositories(
		    $login: String!
		    $from: DateTime!
		    $to: DateTime!
		) {
		    user(login: $login) {
		        contributionsCollection(from: $from, to: $to) {
		            commitContributionsByRepository(maxRepositories: 100) {
		                repository {
		                    nameWithOwner
		                    owner { login }
		                    name
		                }
		            }
					pullRequestContributionsByRepository(maxRepositories: 100) {
					    repository {
					        nameWithOwner
					        owner { login }
					        name
					    }
					}
					issueContributionsByRepository(maxRepositories: 100) {
					    repository {
					        nameWithOwner
					        owner { login }
					        name
					    }
					}
				}
		    }
		}
		""";

	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		var commits = new List<GitHubCommitResponse>();
		var prs = new List<GitHubSearchItemResponse>();
		var issues = new List<GitHubSearchItemResponse>();
		foreach (var username in Config.Usernames)
		{
			var contributionsResult = await GetContributions(input, username);
			if (contributionsResult.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(contributionsResult.Error);
			}

			var contributions = contributionsResult.Value.Data.UserData.Contributions;

			var commitContribs = contributions.Commits.Select(x => x.Data).ToList();
			var commitsResult = await GetCommits(input, commitContribs, username);
			if (commitsResult.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(commitsResult.Error);
			}
			commits.AddRange(commitsResult.Value);

			var prContribs = contributions.Prs.Select(x => x.Data).ToList();
			var prsResult = await GetPrs(input, prContribs, username);
			if (prsResult.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(prsResult.Error);
			}
			prs.AddRange(prsResult.Value);

			var issueContribs = contributions.Issues.Select(x => x.Data).ToList();
			var issuesResult = await GetIssues(input, issueContribs, username);
			if (issuesResult.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(issuesResult.Error);
			}
			issues.AddRange(issuesResult.Value);
		}

		foreach (var commit in commits)
		{
			history.Add(new HistoryItem
			{
				Timestamp = new Timestamp(commit.Commit.Committed.Date, TimestampPrecision.Second),
				Site = "GitHub",
				Description =
					"Committed to " +
					Fmt.Prim(commit.RepoOwner + "/" + commit.RepoName) +
					" - " +
					Fmt.Usr(commit.Commit.Message)
			});
		}
		foreach (var pr in prs)
		{
			history.Add(new HistoryItem
			{
				Timestamp = new Timestamp(pr.CreatedAt, TimestampPrecision.Second),
				Site = "GitHub",
				Description =
					"Created PR " + Fmt.Sec("#" + pr.Number) +
					" on " + Fmt.Prim(pr.Repo) +
					" - " + Fmt.Ter(pr.Title) +
					" - " + Fmt.Usr(pr.Body)
			});
		}
		foreach (var issue in issues)
		{
			history.Add(new HistoryItem
			{
				Timestamp = new Timestamp(issue.CreatedAt, TimestampPrecision.Second),
				Site = "GitHub",
				Description =
					"Opened issue " + Fmt.Sec("#" + issue.Number) +
					" on " + Fmt.Prim(issue.Repo) +
					" - " + Fmt.Ter(issue.Title) +
					" - " + Fmt.Usr(issue.Body)
			});
		}

		return history;
	}

	private static async Task<Result<GitHubContributionsResponse>> GetContributions(ProviderInput input, string username)
	{
		var request = new GitHubContributionsQuery
		{
			Query = ContributionsGraphQl,
			Variables = new
			{
				login = username,
				from = input.DateRange.From.ToString("O"),
				to = input.DateRange.To.ToString("O"),
			}
		};
		var result = await WebRequests.Post<GitHubContributionsQuery, GitHubContributionsResponse>(GitHubGraphQlUrl, request, GetRequestSettings());
		if (result.IsError)
		{
			return Result<GitHubContributionsResponse>.Fail("Contributions GraphQL: " + result.Error);
		}
		if (result.Value is null)
		{
			return Result<GitHubContributionsResponse>.Fail("No response from GraphQL");
		}
		return result.Value;
	}

	private static async Task<Result<IReadOnlyCollection<GitHubCommitResponse>>> GetCommits(ProviderInput input, IReadOnlyList<ContributionRepoData> repoData, string username)
	{
		var responses = new List<GitHubCommitResponse>();

		foreach (var repo in repoData)
		{
			var url = new Uri($"https://api.github.com/repos/{repo.FullName}/commits");
			var queryParams = new Dictionary<string, string>
			{
				{ "author", username },
				{ "since", input.DateRange.From.ToString("O") },
				{ "until", input.DateRange.To.ToString("O") },
				{ "per_page", "100" },
				{ "page", "1" }
			};
			var result = await WebRequests.Get<IReadOnlyCollection<GitHubCommitResponse>>(url, queryParams, GetRequestSettings());
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<GitHubCommitResponse>>.Fail($"{repo.FullName}: commits: {result.Error}");
			}
			if (result.Value != null)
			{
				responses.AddRange(result.Value);
			}
		}

		return responses;
	}

	private static async Task<Result<IReadOnlyCollection<GitHubSearchItemResponse>>> GetPrs(ProviderInput input, IReadOnlyList<ContributionRepoData> repoData, string username, int page = 1)
	{
		var responses = new List<GitHubSearchItemResponse>();

		foreach (var repo in repoData)
		{
			var url = new Uri($"https://api.github.com/search/issues");
			var queryParams = new Dictionary<string, string>
			{
				["q"] = string.Join(" ", [
					$"repo:{repo.FullName}",
					$"author:{username}",
					"type:pr",
					$"created:{input.DateRange.From:yyyy-MM-dd}..{input.DateRange.To:yyyy-MM-dd}",
				])
			};
			var result = await WebRequests.Get<GitHubSearchResponse>(url, queryParams, GetRequestSettings());
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<GitHubSearchItemResponse>>.Fail($"{repo.FullName}: PRs: {result.Error}");
			}
			if (result.Value != null)
			{
				var prs = result.Value.Items.Where(x =>
					input.DateRange.Contains(x.CreatedAt)
				);
				responses.AddRange(prs);
			}
		}

		return responses;
	}

	private static async Task<Result<IReadOnlyCollection<GitHubSearchItemResponse>>> GetIssues(ProviderInput input, IReadOnlyList<ContributionRepoData> repoData, string username)
	{
		var responses = new List<GitHubSearchItemResponse>();

		foreach (var repo in repoData)
		{
			var url = new Uri($"https://api.github.com/search/issues");
			var queryParams = new Dictionary<string, string>
			{
				["q"] = string.Join(" ", [
					$"repo:{repo.FullName}",
					$"author:{username}",
					"type:issue",
					$"created:{input.DateRange.From:yyyy-MM-dd}..{input.DateRange.To:yyyy-MM-dd}",
				])
			};
			var result = await WebRequests.Get<GitHubSearchResponse>(url, queryParams, GetRequestSettings());
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<GitHubSearchItemResponse>>.Fail($"{repo.FullName}: issues: {result.Error}");
			}
			if (result.Value != null)
			{
				var issues = result.Value.Items.Where(x =>
					input.DateRange.Contains(x.CreatedAt)
				);
				responses.AddRange(issues);
			}
		}

		return responses;
	}

	private static Action<HttpClient> GetRequestSettings()
		=> client =>
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvVarStore.GitHubToken);
		};
}
