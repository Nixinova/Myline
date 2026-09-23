using Microsoft.Data.Sqlite;
using Myline.Core.Common;

namespace Myline.Repository.Database;

public class Database
{
	private static readonly string DatabaseFile = Path.Combine(AppData.DataFolder, "Cache.db");

	private readonly string _connectionString = new SqliteConnectionStringBuilder
	{
		DataSource = DatabaseFile
	}.ToString();

	public SqliteConnection CreateConnection()
	{
		return new SqliteConnection(_connectionString);
	}
}
