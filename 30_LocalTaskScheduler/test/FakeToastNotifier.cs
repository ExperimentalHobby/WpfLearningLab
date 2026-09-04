using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のトースト通知を表示しない<see cref="IToastNotifier"/>実装。
/// </summary>
public class FakeToastNotifier : IToastNotifier
{
	/// <summary><see cref="Show"/>に渡された(title, message)の一覧。</summary>
	public List<(string Title, string Message)> ShownNotifications { get; } = [];

	/// <inheritdoc/>
	public void Show(string title, string message) => ShownNotifications.Add((title, message));
}
