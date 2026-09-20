using Myline.Core.Configuration.Interfaces;

namespace Myline.Core.Configuration;

public class WikiProviderConfiguration : IConfiguration
{
	public string ConfigFile { get; } = Configuration.GetFilePath("WikiProvider.txt");

	public async Task<IReadOnlyCollection<Uri>> GetWikiApiUrls()
	{
		await Configuration.InitFile(ConfigFile, DefaultWikiApiUrls);
		var contents = await Configuration.ReadFile(ConfigFile);
		var uris = contents.Select(ParseLine)
			.Where(x => x != null).Cast<Uri>()
			.ToList();
		return uris;
	}

	private static Uri? ParseLine(string line)
	{
		try
		{
			return new Uri(line);
		}
		catch (Exception err)
			when (err is ArgumentNullException or UriFormatException)
		{
			return null;
		}
	}

	private static List<string> DefaultWikiApiUrls =>
	[
		"https://en.wikipedia.org/w/api.php"
	];
}
