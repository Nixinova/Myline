namespace Myline.Core.Common;

public static class AppData
{
	private static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

	public static readonly string DataFolder = Path.Combine(AppDataFolder, "Myline");
}
