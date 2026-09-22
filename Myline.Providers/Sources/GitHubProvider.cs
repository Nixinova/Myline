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

		var contributionsResult = await GetContributions(input);
		if (contributionsResult.IsError)
		{
			return Result<IReadOnlyCollection<HistoryItem>>.Fail(contributionsResult.Error);
		}
		var commits = GetCommits(contributionsResult.Value);

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}

	private object GetCommits(GitHubContributionsResponse contributions)
	{
		throw new NotImplementedException();
	}

	private async Task<Result<GitHubContributionsResponse>> GetContributions(ProviderInput input)
	{
		var request = new GitHubContributionsQuery
		{
			Query = ContributionsGraphQl,
			Variables = new
			{
				login = Config.Username,
				from = input.DateRange.From.ToString("O"),
				to = input.DateRange.To.ToString("O"),
			}
		};
		var result = await WebRequests.Post<GitHubContributionsQuery, GitHubContributionsResponse>(GitHubGraphQlUrl, request);
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
}
