using System.Net.Http.Json;
using System.Web;
using Myline.Core.Models;

namespace Myline.Core.Utilities;

public static class WebRequests
{
	public static HttpClient Init()
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("Myline/0.0");
		return client;
	}

	public static async Task<Result<TModel?>> Get<TModel>(Uri url, Dictionary<string, string> queryParams, Action<HttpClient>? lambda = null)
	{
		var client = Init();
		lambda?.Invoke(client);

		var queryParamString = "?" + string.Join("&",
			queryParams.Select(x =>
				HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)
			)
		);
		var result = await client.GetAsync(url + queryParamString);
		if (!result.IsSuccessStatusCode)
		{
			return Result<TModel?>.Fail(result.StatusCode + " " + result.ReasonPhrase);
		}

		TModel? content;
		try
		{
			content = await result.Content.ReadFromJsonAsync<TModel>();
		}
		catch (Exception err)
		{
			return Result<TModel?>.Fail(err.Message);
		}
		return Result<TModel?>.Ok(content);
	}

	public static async Task<Result<TModel?>> Post<TBody, TModel>(Uri url, TBody body, Action<HttpClient>? lambda = null)
	{
		var client = Init();
		lambda?.Invoke(client);

		var result = await client.PostAsJsonAsync(url, body);
		if (!result.IsSuccessStatusCode)
		{
			return Result<TModel?>.Fail(result.StatusCode + " " + result.ReasonPhrase);
		}

		TModel? content;
		try
		{
			content = await result.Content.ReadFromJsonAsync<TModel>();
		}
		catch (Exception err)
		{
			return Result<TModel?>.Fail(err.Message);
		}
		return Result<TModel?>.Ok(content);
	}
}
