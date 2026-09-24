using System.Net.Http.Headers;
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
		                    owner {
		                        login
		                    }
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

		var commits = new List<GitHubCommitsResponse>();
		foreach (var username in Config.Usernames)
		{
			var contributionsResult = await GetContributions(input, username);
			if (contributionsResult.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(contributionsResult.Error);
			}
			var commitsResult = await GetCommits(contributionsResult.Value, input, username);
			if (commitsResult.IsError)
			{
				return Result<IReadOnlyCollection<HistoryItem>>.Fail(commitsResult.Error);
			}
			commits.AddRange(commitsResult.Value);
		}

		foreach (var commit in commits)
		{
			history.Add(new HistoryItem
			{
				Timestamp = new Timestamp(commit.Commit.Committed.Date, TimestampPrecision.Second),
				Site = "GitHub",
				Description =
					"Committed to " +
					commit.RepoOwner + "/" + commit.RepoName +
					" - " +
					commit.Commit.Message
			});
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private async Task<Result<GitHubContributionsResponse>> GetContributions(ProviderInput input, string username)
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
			return Result<GitHubContributionsResponse>.Fail(result.Error);
		}
		if (result.Value is null)
		{
			return Result<GitHubContributionsResponse>.Fail("No response");
		}
		return Result<GitHubContributionsResponse>.Ok(result.Value);
	}

	private async Task<Result<IReadOnlyCollection<GitHubCommitsResponse>>> GetCommits(GitHubContributionsResponse contributions, ProviderInput input, string username)
	{
		var responses = new List<GitHubCommitsResponse>();

		var repos = contributions.Data.UserData.Contributions.Data
			.Select(x => x.Data);
		foreach (var repo in repos)
		{
			var url = new Uri($"https://api.github.com/repos/{repo.Owner.Name}/{repo.Name}/commits");
			var queryParams = new Dictionary<string, string>
			{
				{ "author", username },
				{ "since", input.DateRange.From.ToString("O") },
				{ "until", input.DateRange.To.ToString("O") },
				{ "per_page", "100" },
				{ "page", "1" }
			};
			var result = await WebRequests.Get<IReadOnlyCollection<GitHubCommitsResponse>>(url, queryParams, GetRequestSettings());
			if (result.IsError)
			{
				return Result<IReadOnlyCollection<GitHubCommitsResponse>>.Fail(result.Error);
			}

			if (result.Value != null)
			{
				responses.AddRange(result.Value);
			}
		}

		return Result<IReadOnlyCollection<GitHubCommitsResponse>>.Ok(responses);
	}

	private Action<HttpClient> GetRequestSettings()
		=> client =>
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvVarStore.GitHubToken);
		};
}
