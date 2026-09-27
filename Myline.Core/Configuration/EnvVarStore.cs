using Myline.Core.Configuration.Interfaces;
using Myline.Core.Utilities;

namespace Myline.Core.Configuration;

public class EnvVarStore : IEnvVarStore
{
	public string? GitHubToken { get; private set; }

	private static readonly string EnvFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

	public async Task<IEnvVarStore> Init()
	{
		var contents = await FileSystemHelper.ReadFile(EnvFilePath);
		var envData = Parse(contents);

		GitHubToken = envData["GITHUB_TOKEN"];

		return this;
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
