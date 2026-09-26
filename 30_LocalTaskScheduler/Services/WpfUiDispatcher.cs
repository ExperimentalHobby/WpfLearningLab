using System.Windows;

namespace LocalTaskScheduler.Services;

/// <summary>
/// WPFの<see cref="System.Windows.Threading.Dispatcher"/>を使った<see cref="IUiDispatcher"/>の実装。
/// </summary>
public class WpfUiDispatcher : IUiDispatcher
{
	/// <inheritdoc/>
	public void Invoke(Action action) => Application.Current.Dispatcher.InvokeAsync(action);
}
