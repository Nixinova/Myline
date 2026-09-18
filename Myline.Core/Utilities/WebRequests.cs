using System.Text.Json;
using System.Web;

namespace Myline.Core.Utilities;

public static class WebRequests
{
	private static HttpClient? _client;

	public static void Init()
	{
		_client = new HttpClient();
		_client.DefaultRequestHeaders.UserAgent.ParseAdd("PersonalHistory/0.0");
	}

	public static async Task<TModel?> Get<TModel>(Uri url, Dictionary<string, string> queryParams)
	{
		var queryParamString = "?" + string.Join("&",
			queryParams.Select(x =>
				HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value)
			)
		);
		var result = await _client!.GetAsync(url + queryParamString);
		var strContent = await result.Content.ReadAsStringAsync();
		var objContent = JsonSerializer.Deserialize<TModel>(strContent);
		return objContent;
	}
}
