using System.Globalization;

namespace Myline.Core.Models;

public class Timestamp
{
	public DateTime Time { get; }
	public TimestampPrecision Precision { get; }

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

	public override string ToString()
	{
		var tzOffset = TimeZoneInfo.Local.GetUtcOffset(Time);
		var time = DateTime.SpecifyKind(Time + tzOffset, DateTimeKind.Local);
		return Precision switch
		{
			TimestampPrecision.Year => time.Year.ToString(),
			TimestampPrecision.Month => time.ToString("yyyy-MM", CultureInfo.InvariantCulture),
			TimestampPrecision.Day => time.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
			TimestampPrecision.Hour => time.ToString("yyyy-MM-dd HH", CultureInfo.InvariantCulture),
			TimestampPrecision.Minute => time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
			TimestampPrecision.Second => time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
			_ => ""
		};
	}

	public static Timestamp FromString(string str)
	{
		var index = 0;

		var year = int.Parse(str[index..(index + 4)]);
		index += 4;
		if (index == str.Length)
		{
			return new Timestamp(new DateTime(year, 1, 1), TimestampPrecision.Year);
		}
		index++; // '-'

		var month = int.Parse(str[index..(index + 2)]);
		index += 2;
		if (index == str.Length)
		{
			return new Timestamp(new DateTime(year, month, 1), TimestampPrecision.Month);
		}
		index++; // '-'

		var day = int.Parse(str[index..(index + 2)]);
		index += 2;
		if (index == str.Length)
		{
			return new Timestamp(new DateTime(year, month, day), TimestampPrecision.Day);
		}

		index++; // ' '

		var hour = int.Parse(str[index..(index + 2)]);
		index += 2;
		if (index == str.Length)
		{
			return new Timestamp(new DateTime(year, month, day, hour, 0, 0), TimestampPrecision.Hour);
		}
		index++; // '-'

		var minute = int.Parse(str[index..(index + 2)]);
		index += 2;
		if (index == str.Length)
		{
			return new Timestamp(new DateTime(year, month, day, hour, minute, 0), TimestampPrecision.Minute);
		}
		index++; // '-'

		var second = int.Parse(str[index..(index + 2)]);
		index += 2;
		if (index == str.Length)
		{
			return new Timestamp(new DateTime(year, month, day, hour, minute, second), TimestampPrecision.Second);
		}

		throw new FormatException("Invalid timestamp: " + str);
	}
}
