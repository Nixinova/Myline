using Myline.Core.Models;
using Myline.Providers.Interfaces;
using Myline.Repository;
using Myline.Repository.Interfaces;

namespace Myline.Providers;

public class HistoryProvider(
	IReadOnlyCollection<IProvider> providers,
	ICachedDatesRepository cachedDatesRepository,
	IHistoryRepository historyRepository
) : IProvider
{
	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		var history = new List<HistoryItem>();

		// Check cache first
		var cached = await cachedDatesRepository.IsCachedBetween(input.DateRange);
		if (cached)
		{
			var cachedHistory = await historyRepository.GetHistoryWithinRange(input.DateRange);
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
		await historyRepository.SaveHistoryItems(history);
		await cachedDatesRepository.SaveCachedDates(input.DateRange);

		return Result<IReadOnlyCollection<HistoryItem>>.Ok(history);
	}
}
