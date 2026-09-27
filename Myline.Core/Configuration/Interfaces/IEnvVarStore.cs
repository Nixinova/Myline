namespace Myline.Core.Configuration.Interfaces;

public interface IEnvVarStore
{
	public string? GitHubToken { get; }

	public Task<IEnvVarStore> Init();
}
