using System.Windows.Input;

namespace BehaviorGallery.Commands;

/// <summary>
/// シンプルな <see cref="ICommand"/> 実装。ギャラリーのデモ用途に、
/// 実行処理をコンストラクタで受け取るだけの最小限のコマンドを提供する。
/// </summary>
public class DelegateCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>コマンドを初期化する。</summary>
    /// <param name="execute">実行するデリゲート。</param>
    /// <param name="canExecute">実行可能かどうかを判定するデリゲート(省略時は常に実行可能)。</param>
    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    /// <inheritdoc/>
    public void Execute(object? parameter) => _execute();
}
