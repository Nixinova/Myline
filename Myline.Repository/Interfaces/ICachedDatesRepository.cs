using Myline.Core.Models;

namespace Myline.Repository.Interfaces;

public interface ICachedDatesRepository
{
	public Task<bool> IsCachedBetween(DateRange dateRange);
	public Task SaveCachedDates(DateRange dateRange);
}
