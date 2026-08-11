using System.Windows;

namespace LocalChatApp.Services;

/// <summary>
/// WPFの<see cref="System.Windows.Threading.Dispatcher"/>をラップする<see cref="IUiDispatcher"/>実装。
/// </summary>
public class WpfUiDispatcher : IUiDispatcher
{
	/// <inheritdoc/>
	public void Invoke(Action action)
	{
		Application.Current.Dispatcher.Invoke(action);
	}
}
