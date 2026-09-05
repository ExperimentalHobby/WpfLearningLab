using ExchangeRateMonitor.Services;

namespace ExchangeRateMonitor.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IExchangeRateApiClient"/>のフェイク実装。
/// 呼び出し順にキューへ積んだ応答(成功/失敗)を1つずつ返す。
/// </summary>
public class FakeExchangeRateApiClient : IExchangeRateApiClient
{
	private readonly Queue<Func<Task<decimal>>> _responses = new();

	/// <summary>これまでに<see cref="GetRateAsync"/>が呼ばれた回数。</summary>
	public int CallCount { get; private set; }

	/// <summary>次回の呼び出しで指定したレートを返すよう予約する。</summary>
	public void EnqueueSuccess(decimal rate) => _responses.Enqueue(() => Task.FromResult(rate));

	/// <summary>次回の呼び出しで通信エラーをスローするよう予約する。</summary>
	public void EnqueueFailure() => _responses.Enqueue(() => throw new HttpRequestException("取得に失敗しました。"));

	/// <summary>
	/// 次回の呼び出しでFormatExceptionをスローするよう予約する
	/// (異常なAPIレスポンスでレート値が数値として不正な場合を模擬する)。
	/// </summary>
	public void EnqueueFormatFailure() => _responses.Enqueue(() => throw new FormatException("レート値の形式が不正です。"));

	/// <inheritdoc/>
	public Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency)
	{
		CallCount++;
		if (_responses.Count == 0)
		{
			throw new InvalidOperationException("テストで予約されていない呼び出しが発生しました。");
		}

		return _responses.Dequeue().Invoke();
	}
}
