using Myline.Core.Configuration.Interfaces;
using Myline.Core.Models;
using Myline.Core.Utilities.Interfaces;

namespace Myline.Providers.Interfaces;

public interface IProvider
{
	public Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input);
}

public record ProviderInput
{
	public required DateRange DateRange { get; init; }
}

public record ProviderConstructorInput
{
	public required IWebRequests WebRequests { get; init; }
	public required IEnvVarStore EnvVarStore { get; init; }
	public required IConfigStore ConfigStore { get; init; }
}
