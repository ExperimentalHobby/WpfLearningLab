using RssReader.Models;
using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実通信を行わない<see cref="IRssFeedClient"/>実装。
/// </summary>
public class FakeRssFeedClient : IRssFeedClient
{
	public IReadOnlyList<RssArticle> ArticlesToReturn { get; set; } = [];
	public Exception? ExceptionToThrow { get; set; }
	public TaskCompletionSource? Gate { get; set; }

	public async Task<IReadOnlyList<RssArticle>> FetchAsync(string feedUrl)
	{
		if (Gate is not null)
		{
			await Gate.Task;
		}

		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return ArticlesToReturn;
	}
}
