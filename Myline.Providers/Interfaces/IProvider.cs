using Myline.Core.Models;

namespace Myline.Providers.Interfaces;

public interface IProvider
{
	public Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input);
}

public record ProviderInput
{
	public required Username Username { get; init; }
	public required DateRange DateRange { get; init; }
}
