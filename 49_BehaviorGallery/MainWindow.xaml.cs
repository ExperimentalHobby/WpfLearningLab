using System.Windows;
using System.Windows.Input;
using BehaviorGallery.Commands;

namespace BehaviorGallery;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ICommand SubmitCommand { get; }

    public MainWindow()
    {
        SubmitCommand = new DelegateCommand(OnSubmit);
        InitializeComponent();
        DataContext = this;
    }

    private void OnSubmit()
    {
        SubmitResultTextBlock.Text = $"送信されました: {EnterKeyTextBox.Text}";
    }
}
