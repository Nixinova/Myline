using System.Net.Http.Json;
using System.Web;
using Myline.Core.Models;

namespace Myline.Core.Utilities;

public static class WebRequests
{
	private static HttpClient? _client;
	private static HttpClient Client => _client!;

	public static void Init()
	{
		_client = new HttpClient();
		_client.DefaultRequestHeaders.UserAgent.ParseAdd("PersonalHistory/0.0");
	}

	public static async Task<Result<TModel?>> Get<TModel>(Uri url, Dictionary<string, string> queryParams)
	{
		var queryParamString = "?" + string.Join("&",
			queryParams.Select(x =>
				HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)
			)
		);
		var result = await Client.GetAsync(url + queryParamString);
		if (!result.IsSuccessStatusCode)
		{
			return Result<TModel?>.Fail(result.StatusCode + " " + result.ReasonPhrase);
		}
		var content = await result.Content.ReadFromJsonAsync<TModel>();
		return Result<TModel?>.Ok(content);
	}

	public static async Task<Result<TModel?>> Post<TBody, TModel>(Uri url, TBody body)
	{
		var result = await Client.PostAsJsonAsync(url, body);
		if (!result.IsSuccessStatusCode)
		{
			return Result<TModel?>.Fail(result.StatusCode + " " + result.ReasonPhrase);
		}
		var content = await result.Content.ReadFromJsonAsync<TModel>();
		return Result<TModel?>.Ok(content);
	}
}
