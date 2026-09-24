namespace Myline.Core.Utilities.Extensions;

public static class EnumerableExtensions
{
	public static T SafeMax<T>(this IEnumerable<T> list)
		where T : notnull
	{
		// T?[].Max doesn't throw; T[].Max does
		return list.Cast<T?>().Max()!;
	}

	public static T SafeMin<T>(this IEnumerable<T> list)
		where T : notnull
	{
		// T?[].Min doesn't throw; T[].Min does
		return list.Cast<T?>().Min()!;
	}
}
