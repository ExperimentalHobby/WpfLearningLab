using System.Windows.Input;

namespace ExchangeRateMonitor.ViewModels;

/// <summary>
/// 型付きパラメータを受け取るデリゲートで <see cref="ICommand"/> を実装する汎用コマンドクラス。
/// 削除対象の監視銘柄など、実行時に情報を渡す必要があるコマンドに使う。
/// </summary>
/// <typeparam name="T">コマンドパラメータの型。</typeparam>
public class RelayCommand<T> : ICommand
{
	private readonly Action<T?> _execute;
	private readonly Func<T?, bool>? _canExecute;

	/// <summary>
	/// コマンドを初期化する。
	/// </summary>
	/// <param name="execute">実行するデリゲート。</param>
	/// <param name="canExecute">実行可能かどうかを判定するデリゲート(省略時は常に実行可能)。</param>
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
