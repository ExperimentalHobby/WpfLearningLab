using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using SingleInstanceLauncher.Models;
using SingleInstanceLauncher.Services;

namespace SingleInstanceLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string MutexName = "Local\\WpfLearningLab.SingleInstanceLauncher.Mutex";
    private const string PipeName = "WpfLearningLab.SingleInstanceLauncher.Pipe";

    private SingleInstanceGuard? _guard;
    private CancellationTokenSource? _serverCts;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _guard = new SingleInstanceGuard(MutexName);

        if (!_guard.IsFirstInstance)
        {
            // 2個目以降の起動: 既存インスタンスへ起動引数を送信して即座に終了する。
            try
            {
                var messenger = new PipeMessenger(PipeName);
                messenger.SendMessage(new LaunchMessage(e.Args, DateTime.UtcNow));
            }
            catch (TimeoutException)
            {
                // 既存インスタンスが応答しない場合は送信をあきらめて終了する。
            }
            catch (IOException)
            {
                // パイプ接続に失敗した場合も同様に終了する。
            }

            _guard.Dispose();
            Shutdown();
            return;
        }

        // 最初のインスタンス: 通常起動し、以後の起動要求を受け付けるサーバーを開始する。
        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();

        _serverCts = new CancellationTokenSource();
        var server = new PipeMessenger(PipeName);
        _ = RunServerAsync(server, mainWindow, _serverCts.Token);
    }

    /// <summary>
    /// <see cref="PipeMessenger.StartServerAsync"/>をfire-and-forgetで呼び出すためのラッパー。
    /// <see cref="PipeMessenger.StartServerAsync"/>自体は既知の例外を内部で処理しているが、
    /// fire-and-forget呼び出しである以上、万一の想定外の例外がアプリ全体をクラッシュさせない
    /// よう最終防御境界として捕捉する。
    /// </summary>
    private static async Task RunServerAsync(PipeMessenger server, MainWindow mainWindow, CancellationToken cancellationToken)
    {
        try
        {
            await server.StartServerAsync(
                message => mainWindow.Dispatcher.Invoke(() => mainWindow.OnLaunchMessageReceived(message)),
                cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"起動待受サーバーで予期しないエラーが発生しました: {ex.Message}");
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serverCts?.Cancel();
        _guard?.Dispose();
        base.OnExit(e);
    }
}
