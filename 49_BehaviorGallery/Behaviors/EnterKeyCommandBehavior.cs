using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace BehaviorGallery.Behaviors;

/// <summary>
/// アタッチした要素でEnterキーが押されたときに指定した <see cref="ICommand"/> を実行するビヘイビア。
/// </summary>
public class EnterKeyCommandBehavior : Behavior<UIElement>
{
    /// <summary>Enterキー押下時に実行するコマンド。</summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command), typeof(ICommand), typeof(EnterKeyCommandBehavior));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        base.OnDetaching();
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (Command?.CanExecute(null) == true)
        {
            Command.Execute(null);
        }
    }
}
