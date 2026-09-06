using System.IO.Pipes;
using SingleInstanceLauncher.Models;
using SingleInstanceLauncher.Services;

namespace SingleInstanceLauncher.Tests;

/// <summary>
/// <see cref="PipeMessenger"/> のテスト。実際のNamed Pipesを使って検証する。
/// </summary>
public class PipeMessengerTests
{
    private static string CreatePipeName() => $"SingleInstanceLauncherTests_{Guid.NewGuid():N}";

    /// <summary>
    /// パス条件: StartServerAsyncを起動した状態でSendMessageすると、
    /// 受信コールバックが正しい内容で呼ばれること
    /// </summary>
    [Fact]
    public async Task SendMessage_StartServerAsync側の受信コールバックが正しい内容で呼ばれる()
    {
        var pipeName = CreatePipeName();
        var server = new PipeMessenger(pipeName);
        using var cts = new CancellationTokenSource();
        LaunchMessage? received = null;
        var tcs = new TaskCompletionSource();

        var serverTask = server.StartServerAsync(message =>
        {
            received = message;
            tcs.TrySetResult();
        }, cts.Token);

        await Task.Delay(100); // サーバーが待受開始するのを待つ
        var sentAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        new PipeMessenger(pipeName).SendMessage(new LaunchMessage(new[] { "--flag" }, sentAt));

        await Task.WhenAny(tcs.Task, Task.Delay(2000));
        cts.Cancel();
        await Task.WhenAny(serverTask, Task.Delay(1000));

        Assert.NotNull(received);
        Assert.Equal(new[] { "--flag" }, received!.Arguments);
        Assert.Equal(sentAt, received.SentAtUtc);
    }

    /// <summary>
    /// パス条件: 不正な(JSONとして壊れた)データを直接パイプに送信しても、サーバーがクラッシュせず
    /// 待受を継続し、その後の正常なメッセージを引き続き受信できること
    /// (パイプ関連の例外・不正な受信データが捕捉されず、fire-and-forgetで未処理タスク例外に
    /// なっていた不具合の回帰テスト)。
    /// </summary>
    [Fact]
    public async Task 不正なデータを受信してもサーバーは継続し後続の正常なメッセージを受信できる()
    {
        var pipeName = CreatePipeName();
        var server = new PipeMessenger(pipeName);
        using var cts = new CancellationTokenSource();
        LaunchMessage? received = null;
        var tcs = new TaskCompletionSource();

        var serverTask = server.StartServerAsync(message =>
        {
            received = message;
            tcs.TrySetResult();
        }, cts.Token);

        await Task.Delay(100);

        // 不正なデータを直接送信する(LaunchMessageSerializerを経由しない生のJSON崩れ)。
        using (var badClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
        {
            badClient.Connect(2000);
            using var writer = new StreamWriter(badClient) { AutoFlush = true };
            writer.WriteLine("{ this is not valid json");
        }

        await Task.Delay(200);

        // サーバーがまだ生きていて、次の正常なメッセージを受信できることを確認する。
        var sentAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        new PipeMessenger(pipeName).SendMessage(new LaunchMessage(Array.Empty<string>(), sentAt));

        await Task.WhenAny(tcs.Task, Task.Delay(2000));
        cts.Cancel();
        await Task.WhenAny(serverTask, Task.Delay(1000));

        Assert.False(serverTask.IsFaulted, serverTask.Exception?.ToString());
        Assert.NotNull(received);
        Assert.Equal(sentAt, received!.SentAtUtc);
    }
}
