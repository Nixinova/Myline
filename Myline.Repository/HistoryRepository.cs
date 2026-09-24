using Microsoft.Data.Sqlite;
using Myline.Core.Models;
using Myline.Repository.Database;

namespace Myline.Repository;

public static class HistoryRepository
{
	public static async Task<IReadOnlyList<HistoryItem>> GetHistoryForSiteWithinRange(string site, DateRange dateRange)
	{
		await using var connection = DataStore.Database.CreateConnection();
		await connection.OpenAsync();

		await using var command = connection.CreateCommand();

		command.CommandText =
			"""
			select Timestamp, Desc from Cached
			where Site = @site and Timestamp between @dateFrom and @dateTo
			""";
		command.Parameters.AddWithValue("@site", site);
		command.Parameters.AddWithValue("@dateFrom", dateRange.From.ToString("yyyy-MM-dd"));
		command.Parameters.AddWithValue("@dateTo", dateRange.To.ToString("yyyy-MM-dd"));

		var reader = await command.ExecuteReaderAsync();
		var entries = new List<HistoryItem>();
		while (await reader.ReadAsync())
		{
			var timestamp = Timestamp.FromString(reader.GetString(0));
			var description = reader.GetString(1);
			entries.Add(new HistoryItem
			{
				Timestamp = timestamp,
				Site = site,
				Description = description
			});
		}

		return entries;
	}

	public static async Task SaveHistoryItems(IReadOnlyList<HistoryItem> history)
	{
		await using var connection = DataStore.Database.CreateConnection();
		await connection.OpenAsync();

		await using var transaction = await connection.BeginTransactionAsync();

		await using var command = connection.CreateCommand();
		command.Transaction = (SqliteTransaction)transaction;

		command.CommandText =
			"""
			insert or ignore into Cache (Timestamp, Site, Description)
			values (@time, @site, @desc);
			""";
		command.Parameters.Add("@time", SqliteType.Text);
		command.Parameters.Add("@site", SqliteType.Text);
		command.Parameters.Add("@desc", SqliteType.Text);

		foreach (var item in history)
		{
			command.Parameters["@time"].Value = item.Timestamp.ToString();
			command.Parameters["@site"].Value = item.Site;
			command.Parameters["@desc"].Value = item.Description;
			await command.ExecuteNonQueryAsync();
		}

		await transaction.CommitAsync();
	}
}
