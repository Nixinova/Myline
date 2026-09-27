using Myline.Core.Models;

namespace Myline.Repository.Interfaces;

public interface IHistoryRepository
{
	public Task<IReadOnlyList<HistoryItem>> GetHistoryWithinRange(DateRange dateRange);
	public Task SaveHistoryItems(IReadOnlyList<HistoryItem> history);
}
