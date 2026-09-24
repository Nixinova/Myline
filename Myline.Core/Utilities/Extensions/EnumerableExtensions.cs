namespace Myline.Core.Utilities.Extensions;

public static class EnumerableExtensions
{
	public static T? SafeMax<T>(this IEnumerable<T> list)
	{
		try
		{
			return list.Max();
		}
		catch (Exception)
		{
			return default;
		}
	}

	public static T? SafeMin<T>(this IEnumerable<T> list)
	{
		try
		{
			return list.Min();
		}
		catch (Exception)
		{
			return default;
		}
	}
}
