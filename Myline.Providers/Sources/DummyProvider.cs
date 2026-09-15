using Myline.Core.Models;
using Myline.Providers.Interfaces;

namespace Myline.Providers.Sources;

public class DummyProvider : IProvider
{
	public async Task<Result<IReadOnlyCollection<HistoryItem>>> CollectHistory(ProviderInput input)
	{
		await Task.Delay(0);
		return Result<IReadOnlyCollection<HistoryItem>>.Ok(new List<HistoryItem>
		{
			new()
			{
				Timestamp = new Timestamp(new DateTime(2026, 9, 1), TimestampPrecision.Day),
				Title = "Created entry Foo",
				Type = HistoryType.Edit
			}
		});
	}
}
