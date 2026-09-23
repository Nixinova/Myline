using Myline.Core.Configuration;
using Myline.Core.Models;
using Myline.Core.Utilities;
using Myline.Display.Mappers;
using Myline.Providers;
using Myline.Providers.Interfaces;
using Myline.Providers.Sources;
using Myline.Repository.Database;

namespace Myline.Display;

public class Program
{
	static async Task Main(string[] args)
	{
		await EnvVarStore.Init();
		await ConfigStore.Init();
		await DataStore.Init();
		WebRequests.Init();

		var sourceProviders = CreateProviders().ToList();
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
		if (args.Length is not (1 or 2))
		{
			Console.WriteLine("Usage: Myline.Display.exe <fromTime> [<toTime>]");
			Console.WriteLine("Make sure to add your settings in %AppData%/Myline/Config.json before use!");
			return Result<ProviderInput>.Fail("No arguments provided");
		}

		var fromTime = DateTime.Parse(args[0]);
		var toTime = args.Length < 2 ? fromTime.AddDays(1) : DateTime.Parse(args[1]);
		var fromTz = TimeZoneInfo.Local.GetUtcOffset(fromTime);
		var toTz = TimeZoneInfo.Local.GetUtcOffset(toTime);
		fromTime = DateTime.SpecifyKind(fromTime - fromTz, DateTimeKind.Utc);
		toTime = DateTime.SpecifyKind(toTime - toTz,  DateTimeKind.Utc);
		return Result<ProviderInput>.Ok(new ProviderInput
		{
			DateRange = new DateRange(fromTime, toTime),
		});
	}

	private static IEnumerable<IProvider> CreateProviders()
	{
		yield return new GitHubProvider();
		yield return new HowLongToBeatProvider();
		yield return new LastFmProvider();
		yield return new WikiProvider();
	}
}
