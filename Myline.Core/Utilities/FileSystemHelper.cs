namespace Myline.Core.Utilities;

public static class FileSystemHelper
{
	public static void TouchFile(string file)
	{
		if (!File.Exists(file))
		{
			File.Create(file).Dispose();
		}
	}

	public static async Task<string> ReadFile(string file)
	{
		TouchFile(file);

		using var reader = new StreamReader(file);

		return await reader.ReadToEndAsync();
	}

	public static async Task InitFile(string file, string contents)
	{
		await File.WriteAllTextAsync(file, contents + Environment.NewLine);
	}

	public static async Task InitFile(string file, string[] lines)
	{
		await File.WriteAllTextAsync(file,
			string.Join(Environment.NewLine, lines) + Environment.NewLine);
	}
}
