using Myline.Core.Utilities;

namespace Myline.Core.Configuration;

public static class EnvVarStore
{
	public static string? GitHubToken;

	private static readonly string EnvFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

	public static async Task Init()
	{
		var contents = await FileSystemHelper.ReadFile(EnvFilePath);
		var envData = Parse(contents);

		GitHubToken = envData["GITHUB_TOKEN"];
	}

	private static Dictionary<string, string> Parse(string contents)
	{
		return contents
			.Split(Environment.NewLine)
			.ToDictionary(
				x => x.Split('=')[0],
				x => string.Join('=', x.Split('=')[1..])
			);
	}
}
