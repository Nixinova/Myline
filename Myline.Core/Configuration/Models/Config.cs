namespace Myline.Core.Configuration.Models;

public record Config
{
	public GitHubConfig GitHubConfig { get; init; } = new();
	public HltbConfig HltbConfig { get; init; } = new();
	public LastFmConfig LastFmConfig { get; init; } = new();
	public WikiConfig WikiConfig { get; init; } = new();
}

public record GitHubConfig
{
	public string[] Usernames { get; init; } = [];
}

public class HltbConfig
{
	public int[] UserIds { get; init; } = [];
}

public class LastFmConfig
{
	public string[] Usernames { get; init; } = [];
}

public class WikiConfig
{
	public string[] ApiUrls { get; init; } = [];
	public IReadOnlyDictionary<string, string[]> UsernamesPerWikiDomain { get; init; } = new Dictionary<string, string[]>();
}
