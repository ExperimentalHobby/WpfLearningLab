using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using SingleInstanceLauncher.Models;

namespace SingleInstanceLauncher.Services;

/// <summary>
/// Named Pipesを使って <see cref="LaunchMessage"/> を1行のJSONとしてやり取りする。
/// </summary>
public class PipeMessenger
{
    private readonly string _pipeName;

    public PipeMessenger(string pipeName)
    {
        _pipeName = pipeName;
    }

    /// <summary>
    /// 接続を待ち受け、メッセージを受信するたびに <paramref name="onMessageReceived"/> を呼び出す
    /// ループを開始する。<paramref name="cancellationToken"/> がキャンセルされるまで継続する。
    /// </summary>
    public async Task StartServerAsync(Action<LaunchMessage> onMessageReceived, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var server = new NamedPipeServerStream(
                    _pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync(cancellationToken);

                using var reader = new StreamReader(server);
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line != null)
                {
                    onMessageReceived(LaunchMessageSerializer.Deserialize(line));
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセル時はループを抜ける。
                break;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException)
            {
                // このループはfire-and-forgetで呼び出されるため、ここで捕捉しないと
                // 1回の接続失敗・不正な受信データが未処理のタスク例外としてアプリ全体を
                // クラッシュさせうる。1回分の失敗として扱い、待受自体は継続する。
                Debug.WriteLine($"起動待受サーバーでエラーが発生しました: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 既存インスタンスへ接続し、メッセージを1行のJSONとして送信する。
    /// </summary>
    public void SendMessage(LaunchMessage message, int timeoutMs = 2000)
    {
        using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
        client.Connect(timeoutMs);

        using var writer = new StreamWriter(client) { AutoFlush = true };
        writer.WriteLine(LaunchMessageSerializer.Serialize(message));
    }
}
