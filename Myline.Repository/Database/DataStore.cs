namespace Myline.Repository.Database;

public static class DataStore
{
	private static Database _database = null!;

	private const string DbInitCmd =
		"""
		create table if not exists History (
			Id integer primary key autoincrement,
			Timestamp text not null,
			Site text not null,
			Desc text not null
		);

		create table if not exists Cached (
			Site text not null,
			Date text not null
		);

		create index if not exists idx_History_Timestamp
		    ON History (Timestamp);

		create index if not exists idx_History_Site
		    ON History (Site);

		create index if not exists idx_Cached_Site
		    ON Cached (Site);

		create index if not exists idx_Cached_Date
		    ON Cached (Date);
		""";

	public static async Task Init()
	{
		_database = new Database();

		await using var connection = _database.CreateConnection();
		await connection.OpenAsync();

		await using var command = connection.CreateCommand();
		command.CommandText =  DbInitCmd;
		await command.ExecuteNonQueryAsync();
	}
}
