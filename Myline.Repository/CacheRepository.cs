using Microsoft.Data.Sqlite;
using Myline.Core.Models;
using Myline.Repository.Database;

namespace Myline.Repository;

public static class CacheRepository
{
	public static async Task<IReadOnlyList<DateOnly>> GetCacheDays(string site, DateRange dateRange)
	{
		await using var connection = DataStore.Database.CreateConnection();
		await connection.OpenAsync();

		await using var command = connection.CreateCommand();

		command.CommandText =
			"""
			select Date from Cached
			where Site = @site and Date between @dateFrom and @dateTo
			""";
		command.Parameters.AddWithValue("@site", site);
		command.Parameters.AddWithValue("@dateFrom", dateRange.From.ToString("yyyy-MM-dd"));
		command.Parameters.AddWithValue("@dateTo", dateRange.To.ToString("yyyy-MM-dd"));

		var reader = await command.ExecuteReaderAsync();
		var dates = new List<DateOnly>();
		while (await reader.ReadAsync())
		{
			var dateRaw = reader.GetString(0);
			var date = DateOnly.Parse(dateRaw);
			dates.Add(date);
		}

		return dates;
	}

	public static async Task SaveCacheDays(string site, DateRange dateRange)
	{
		await using var connection = DataStore.Database.CreateConnection();
		await connection.OpenAsync();

		await using var transaction = await connection.BeginTransactionAsync();

		await using var command = connection.CreateCommand();
		command.Transaction = (SqliteTransaction)transaction;

		command.CommandText =
			"""
			insert or ignore into Cache (Site, Date)
			values (@site, @date);
			""";
		command.Parameters.Add("@site", SqliteType.Text);
		command.Parameters.Add("@date", SqliteType.Text);

		for (var date = dateRange.From.Date; date <= dateRange.To.Date; date = date.AddDays(1))
		{
			command.Parameters["@site"].Value = site;
			command.Parameters["@date"].Value = date.ToString("yyyy-MM-dd");
			await command.ExecuteNonQueryAsync();
		}

		await transaction.CommitAsync();
	}
}
