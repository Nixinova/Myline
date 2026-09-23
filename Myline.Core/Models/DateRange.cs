namespace Myline.Core.Models;

public class DateRange(DateTime from, DateTime to)
{
	public DateTime From { get; } = from;
	public DateTime To { get; } = to;

	public bool Contains(DateTime date)
	{
		return From <= date && date <= To;
	}

	public bool Contains(DateOnly date)
	{
		return DateOnly.FromDateTime(From) <= date && date <= DateOnly.FromDateTime(To);
	}
}
