using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のトースト通知を表示しない<see cref="IToastNotifier"/>実装。
/// </summary>
public class FakeToastNotifier : IToastNotifier
{
	/// <summary><see cref="Show"/>に渡された(title, message)の一覧。</summary>
	public List<(string Title, string Message)> ShownNotifications { get; } = [];

	/// <summary>設定すると<see cref="Show"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <inheritdoc/>
	public void Show(string title, string message)
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		ShownNotifications.Add((title, message));
	}
}
