using System.Windows.Input;

namespace FileOrganizer.ViewModels;

/// <summary>
/// 非同期処理を実行する <see cref="ICommand"/> 実装。
/// 実行中は多重実行を防ぐために <see cref="CanExecute"/> がfalseを返すようにする。
/// </summary>
public class AsyncRelayCommand : ICommand
{
	private readonly Func<Task> _execute;
	private readonly Func<bool>? _canExecute;
	private bool _isExecuting;

	/// <summary>
	/// コマンドを初期化する。
	/// </summary>
	/// <param name="execute">実行する非同期デリゲート。</param>
	/// <param name="canExecute">実行可能かどうかを判定するデリゲート(省略時は常に実行可能)。</param>
	public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	/// <inheritdoc/>
	public event EventHandler? CanExecuteChanged;

	/// <inheritdoc/>
	public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

	/// <inheritdoc/>
	public async void Execute(object? parameter)
	{
		_isExecuting = true;
		RaiseCanExecuteChanged();
		try
		{
			await _execute();
		}
		finally
		{
			_isExecuting = false;
			RaiseCanExecuteChanged();
		}
	}

	/// <summary>
	/// <see cref="CanExecuteChanged"/> を発火し、コマンドの実行可否をUIに再評価させる。
	/// </summary>
	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
