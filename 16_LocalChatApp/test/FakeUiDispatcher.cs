using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="LocalChatApp.ViewModels.MainViewModel"/> のテスト用に、実際のWPF Dispatcherを使わず同期的に実行する<see cref="IUiDispatcher"/>実装。
/// </summary>
public class FakeUiDispatcher : IUiDispatcher
{
	/// <inheritdoc/>
	public void Invoke(Action action) => action();
}
