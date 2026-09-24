using Myline.Core.Models;
using Myline.Repository.Database;

namespace Myline.Repository;

public static class CachedDatesRepository
{
	public static async Task<bool> IsCachedBetween(DateRange dateRange)
	{
		await using var connection = DataStore.Database.CreateConnection();
		await connection.OpenAsync();

		await using var command = connection.CreateCommand();

		command.CommandText =
			"""
			select TimestampFrom, TimestampTo from CachedDates
			where @dateTimeFrom >= TimestampFrom
			and @dateTimeTo <= TimestampTo
			limit 1
			""";
		command.Parameters.AddWithValue("@dateTimeFrom", dateRange.From.ToString("u"));
		command.Parameters.AddWithValue("@dateTimeTo", dateRange.To.ToString("u"));

		var reader = await command.ExecuteReaderAsync();
		return reader.HasRows;
	}

	public static async Task SaveCachedDates(DateRange dateRange)
	{
		await using var connection = DataStore.Database.CreateConnection();
		await connection.OpenAsync();

		await using var command = connection.CreateCommand();
		command.CommandText =
			"""
			insert into CachedDates (TimestampFrom, TimestampTo)
			values (@dateTimeFrom, @dateTimeTo);
			""";
		command.Parameters.AddWithValue("@dateTimeFrom", dateRange.From.ToString("u"));
		command.Parameters.AddWithValue("@dateTimeTo", dateRange.To.ToString("u"));

		await command.ExecuteNonQueryAsync();
	}
}
