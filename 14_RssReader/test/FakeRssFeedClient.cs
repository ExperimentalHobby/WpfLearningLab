using RssReader.Models;
using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="RssReader.ViewModels.MainViewModel"/> のテスト用に、実通信を行わない<see cref="IRssFeedClient"/>実装。
/// </summary>
public class FakeRssFeedClient : IRssFeedClient
{
	/// <summary><see cref="FetchAsync"/>が返す値(テスト用)。</summary>
	public IReadOnlyList<RssArticle> ArticlesToReturn { get; set; } = [];

	/// <summary>設定すると<see cref="FetchAsync"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <summary>設定すると<see cref="FetchAsync"/>がこのTaskの完了まで待機する(テスト用)。</summary>
	public TaskCompletionSource? Gate { get; set; }

	/// <inheritdoc/>
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
