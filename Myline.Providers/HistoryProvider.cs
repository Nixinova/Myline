using Myline.Core.Models;
using Myline.Providers.Interfaces;
using Myline.Repository;

namespace Myline.Providers;

public class HistoryProvider(IReadOnlyCollection<IProvider> providers) : IProvider
{
	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		// Check cache first
		var cached = await CachedDatesRepository.IsCachedBetween(input.DateRange);
		if (cached)
		{
			var cachedHistory = await HistoryRepository.GetHistoryWithinRange(input.DateRange);
			return Result<IReadOnlyCollection<HistoryItem>>.Ok(cachedHistory);
		}

		// Fetch history entries
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

		// Save to cache
		await HistoryRepository.SaveHistoryItems(history);
		await CachedDatesRepository.SaveCachedDates(input.DateRange);

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}
}
