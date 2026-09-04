using Microsoft.Toolkit.Uwp.Notifications;

namespace LocalTaskScheduler.Services;

/// <summary>
/// <see cref="ToastContentBuilder"/>(Microsoft.Toolkit.Uwp.Notifications)を使う<see cref="IToastNotifier"/>実装。
/// </summary>
public class ToastNotifier : IToastNotifier
{
	/// <inheritdoc/>
	public void Show(string title, string message)
	{
		new ToastContentBuilder()
			.AddText(title)
			.AddText(message)
			.Show();
	}
}
