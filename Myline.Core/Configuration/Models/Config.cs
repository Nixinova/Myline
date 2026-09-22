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
	public string Username { get; init; } = "";
}

public class HltbConfig
{
	public int UserId { get; init; } = 0;
}

public class LastFmConfig
{
	public string Username { get; init; } = "";
}

public class WikiConfig
{
	public string[] ApiUrls { get; init; } = [];
	public Dictionary<string, string> UsernamesPerWikiDomain { get; init; } = new();
}
