using System.Windows.Input;

namespace Simple3DViewer.ViewModels;

/// <summary>
/// 引数を1つ受け取るデリゲートを実行する<see cref="ICommand"/>実装。
/// </summary>
public class RelayCommand<T> : ICommand
{
	private readonly Action<T?> _execute;
	private readonly Func<T?, bool>? _canExecute;

	/// <summary>
	/// コマンドを初期化する。
	/// </summary>
	public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	/// <inheritdoc/>
	public event EventHandler? CanExecuteChanged;

	/// <inheritdoc/>
	public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

	/// <inheritdoc/>
	public void Execute(object? parameter) => _execute((T?)parameter);

	/// <summary>
	/// <see cref="CanExecuteChanged"/> を発火し、コマンドの実行可否をUIに再評価させる。
	/// </summary>
	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
