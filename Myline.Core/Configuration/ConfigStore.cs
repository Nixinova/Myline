using System.Text.Json;
using Myline.Core.Common;
using Myline.Core.Configuration.Models;
using Myline.Core.Utilities;

namespace Myline.Core.Configuration;

public static class ConfigStore
{
	public static Config Config { get; private set; } = new();

	private static readonly string ConfigFilePath = Path.Combine(AppData.DataFolder, "Config.json");

	public static async Task Init()
	{
		var folder = Path.GetDirectoryName(ConfigFilePath)!;
		if (!Directory.Exists(folder))
		{
			Directory.CreateDirectory(folder);
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
