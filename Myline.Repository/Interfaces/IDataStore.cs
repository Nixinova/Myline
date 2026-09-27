namespace Myline.Repository.Interfaces;

public interface IDataStore
{
	public Database.Database Database { get; }

	public Task<IDataStore> Init();
}
