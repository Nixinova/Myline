namespace Myline.Core.Models;

public class Timestamp
{
	public DateTime Time { get; private init; }
	public TimestampPrecision Precision { get; private init; }

	public Timestamp(DateTime time, TimestampPrecision precision)
	{
		Time = time;
		Precision = precision;
	}

	public Timestamp(DateOnly date, TimestampPrecision precision)
	{
		if (precision is TimestampPrecision.Hour or TimestampPrecision.Minute or TimestampPrecision.Second)
		{
			throw new InvalidOperationException("Invalid timestamp precision");
		}
		Time = new DateTime(date.Year, date.Month, date.Day);
		Precision = precision;
	}
}
