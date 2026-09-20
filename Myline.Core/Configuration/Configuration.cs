namespace Myline.Core.Configuration;

public static class Configuration
{
	private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

	private static readonly string ConfigFolder = Path.Combine(AppData, "Myline");

	public static void Create()
	{
		if (!Directory.Exists(ConfigFolder))
		{
			Directory.CreateDirectory(ConfigFolder);
		}
	}

	public static string GetFilePath(string file)
	{
		return Path.Combine(ConfigFolder, file);
	}

	public static void TouchFile(string file)
	{
		if (!File.Exists(file))
		{
			File.Create(file).Dispose();
		}
	}

	public static async Task<IReadOnlyList<string>> ReadFile(string file)
	{
		TouchFile(file);

		using var reader = new StreamReader(file);

		var contents = await reader.ReadToEndAsync();
		return Parse(contents);
	}

	public static async Task InitFile(string file, IEnumerable<string> lines)
	{
		if (File.Exists(file))
		{
			return;
		}

		var contents = string.Join(Environment.NewLine, lines);
		await File.WriteAllTextAsync(file, contents + Environment.NewLine);
	}

	public static IReadOnlyList<string> Parse(string content)
	{
		var lines = new List<string>();

		foreach (var line in content.Split(Environment.NewLine))
		{
			if (string.IsNullOrWhiteSpace(line)) continue;
			if (line.StartsWith('#')) continue;
			lines.Add(line.Trim());
		}

		return lines;
	}
}
