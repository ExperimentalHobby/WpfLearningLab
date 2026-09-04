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

    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();
}
