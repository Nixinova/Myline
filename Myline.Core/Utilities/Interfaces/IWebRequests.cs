using Myline.Core.Models;

namespace Myline.Core.Utilities.Interfaces;

public interface IWebRequests
{
	public Task<Result<TModel?>> Get<TModel>(Uri url, Dictionary<string, string> queryParams, Action<HttpClient>? lambda = null);
	public Task<Result<TModel?>> Post<TBody, TModel>(Uri url, TBody body, Action<HttpClient>? lambda = null);
}
