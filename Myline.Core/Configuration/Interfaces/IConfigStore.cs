using Myline.Core.Configuration.Models;

namespace Myline.Core.Configuration.Interfaces;

public interface IConfigStore
{
	public Config Config { get; }
	public Task<IConfigStore> Init();
}
