using System.Text.Json;
using Myline.Core.Configuration.Models;
using Myline.Core.Utilities;

namespace Myline.Core.Configuration;

public static class ConfigStore
{
	public static Config Config = new();

	private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
	private static readonly string ConfigFolder = Path.Combine(AppData, "Myline");
	private static readonly string ConfigFilePath = Path.Combine(ConfigFolder, "Config.json");

	public static async Task Init()
	{
		if (!Directory.Exists(ConfigFolder))
		{
			Directory.CreateDirectory(ConfigFolder);
		}
		if (!File.Exists(ConfigFilePath))
		{
			var content = JsonSerializer.Serialize(Config,
				new JsonSerializerOptions
				{
					WriteIndented = true
				});
			await FileSystemHelper.InitFile(ConfigFilePath, content);
			Console.WriteLine($"INFO: First-time initialisation has occurred. Please edit {ConfigFilePath} before running Myline.");
			Environment.Exit(1);
		}

		await LoadConfig();
	}

	private static async Task LoadConfig()
	{
		var contents = await FileSystemHelper.ReadFile(ConfigFilePath);
		var configContents = JsonSerializer.Deserialize<Config>(contents);
		if (configContents != null)
		{
			Config = configContents;
		}
	}
}
