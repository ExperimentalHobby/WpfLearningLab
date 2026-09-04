using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using SingleInstanceLauncher.Models;

namespace SingleInstanceLauncher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ObservableCollection<string> _messages = new();

    public MainWindow()
    {
        InitializeComponent();
        MessagesListBox.ItemsSource = _messages;
        ProcessIdTextBlock.Text = $"プロセスID: {Environment.ProcessId}";
    }

    /// <summary>
    /// 2個目以降の起動から送信された <see cref="LaunchMessage"/> を受信したときに呼び出される。
    /// UIスレッドから呼び出すこと。
    /// </summary>
    public void OnLaunchMessageReceived(LaunchMessage message)
    {
        var argsText = message.Arguments.Length > 0 ? string.Join(" ", message.Arguments) : "(引数なし)";
        _messages.Insert(0, $"{message.SentAtUtc.ToLocalTime():HH:mm:ss} - {argsText}");

        BringToForeground();
    }

    private void BringToForeground()
    {
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Show();
        Topmost = true;
        Topmost = false;
        Activate();
    }
}
