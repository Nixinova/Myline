using Myline.Core.Models;
using Myline.Display.Mappers;
using Myline.Providers;
using Myline.Providers.Interfaces;
using Myline.Providers.Sources;

namespace Myline.Display;

public class Program
{
	static async Task Main(string[] args)
	{
		var sourceProviders = GetProviders().ToList();
		var provider = new HistoryProvider(sourceProviders);

		var inputResult = ParseInput(args);
		if (inputResult.IsError)
		{
			Console.WriteLine(inputResult.Error);
			return;
		}

		var historyResult = await provider.CollectHistory(inputResult.Value);
		if (historyResult.IsError)
		{
			Console.WriteLine(historyResult.Error);
			return;
		}

		var viewModel = HistoryItemViewMapper.MapToViewModel(historyResult.Value);

		CliDisplay.DisplayData(viewModel);
	}

	private static Result<ProviderInput> ParseInput(string[] args)
	{
		if (args.Length != 3)
		{
			Console.WriteLine("Usage: Myline.Display.exe <username> <fromTime> <toTime>");
			return Result<ProviderInput>.Fail("No arguments provided");
		}

		var username = new Username(args[0]);
		var fromTime = DateTime.Parse(args[1]);
		var toTime = DateTime.Parse(args[2]);
		return Result<ProviderInput>.Ok(new ProviderInput
		{
			Username = username,
			DateRange = new DateRange(fromTime, toTime),
		});
	}

	private static IEnumerable<IProvider> GetProviders()
	{
		yield return new DummyProvider();
	}
}
