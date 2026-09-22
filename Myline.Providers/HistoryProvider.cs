using Myline.Core.Models;
using Myline.Providers.Interfaces;

namespace Myline.Providers;

public class HistoryProvider(IReadOnlyCollection<IProvider> providers) : IProvider
{
	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		foreach (var provider in providers)
		{
			var collectionResult = await provider.CollectHistory(input);
			if (collectionResult.IsError)
			{
				Console.WriteLine($"Error ({provider}): {collectionResult.Error}");
				continue;
			}
			history.AddRange(collectionResult.Value);
		}

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}
}
