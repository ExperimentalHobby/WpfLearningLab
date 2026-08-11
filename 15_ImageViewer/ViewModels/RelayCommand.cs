using System.Windows.Input;

namespace ImageViewer.ViewModels;

/// <summary>
/// パラメータを取らないデリゲートを受け取って <see cref="ICommand"/> を実装する汎用コマンドクラス。
/// </summary>
public class RelayCommand : ICommand
{
	private readonly Action _execute;
	private readonly Func<bool>? _canExecute;

	/// <summary>
	/// コマンドを初期化する。
	/// </summary>
	/// <param name="execute">実行するデリゲート。</param>
	/// <param name="canExecute">実行可能かどうかを判定するデリゲート(省略時は常に実行可能)。</param>
	public RelayCommand(Action execute, Func<bool>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	/// <inheritdoc/>
	public event EventHandler? CanExecuteChanged;

	/// <inheritdoc/>
	public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

	/// <inheritdoc/>
	public void Execute(object? parameter) => _execute();

	/// <summary>
	/// <see cref="CanExecuteChanged"/> を発火し、コマンドの実行可否をUIに再評価させる。
	/// </summary>
	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
