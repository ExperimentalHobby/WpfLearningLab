using LogStreamAggregator.Services;

namespace LogStreamAggregator.Tests.Fakes;

/// <summary>
/// UIスレッドを持たないテスト環境向けに、呼び出しをそのまま同期実行する<see cref="IUiDispatcher"/>のフェイク。
/// </summary>
internal class ImmediateUiDispatcher : IUiDispatcher
{
	public void Invoke(Action action) => action();
}
