namespace Myline.Repository.Database;

public static class DataStore
{
	public static Database Database { get; private set; } = null!;

	private const string DbInitCmd =
		"""
		create table if not exists History (
			-- Timestamp stored as TimeStamp::ToString()
			Timestamp text not null primary key,
			Site text not null,
			Description text not null
		);

		create table if not exists CachedDates (
			-- Timestamps stored as DateTime::ToString("u")
			TimestampFrom text not null,
			TimestampTo text not null,
			primary key (TimestampFrom, TimestampTo)
		);
		""";

	public static async Task Init()
	{
		Database = new Database();

		await using var connection = Database.CreateConnection();
		await connection.OpenAsync();

		await using var command = connection.CreateCommand();
		command.CommandText =  DbInitCmd;
		await command.ExecuteNonQueryAsync();
	}
}
