namespace Myline.Core.Models;

public readonly record struct DateRange(DateTime From, DateTime To)
{
	public bool Contains(DateTime date)
	{
		return From <= date && date <= To;
	}

	public bool Contains(DateOnly date)
	{
		return DateOnly.FromDateTime(From) <= date && date <= DateOnly.FromDateTime(To);
	}
}
