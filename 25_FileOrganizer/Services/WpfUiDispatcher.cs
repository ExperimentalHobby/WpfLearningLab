using System.Windows;

namespace FileOrganizer.Services;

/// <summary>
/// WPFの<see cref="Application.Dispatcher"/>を使った<see cref="IUiDispatcher"/>の実装。
/// </summary>
public class WpfUiDispatcher : IUiDispatcher
{
	/// <inheritdoc/>
	public void Invoke(Action action) => Application.Current.Dispatcher.Invoke(action);
}
