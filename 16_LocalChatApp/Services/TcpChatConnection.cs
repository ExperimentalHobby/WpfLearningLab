using System.IO;
using System.Net.Sockets;
using System.Text;

namespace LocalChatApp.Services;

/// <summary>
/// <see cref="TcpClient"/>のNetworkStreamをラップし、改行区切りのテキストメッセージを
/// 非同期に送受信する<see cref="IChatConnection"/>実装。
/// 生成と同時にバックグラウンドで読み取りループを開始する。
/// </summary>
public class TcpChatConnection : IChatConnection
{
	private readonly TcpClient _client;
	private readonly StreamReader _reader;
	private readonly StreamWriter _writer;
	private bool _disposed;

	/// <summary>
	/// 接続済みの<see cref="TcpClient"/>をラップし、読み取りループを開始する。
	/// </summary>
	/// <param name="client">接続済み(Connect/AcceptTcpClientAsync済み)のTcpClient。</param>
	public TcpChatConnection(TcpClient client)
	{
		_client = client;
		var stream = client.GetStream();
		_reader = new StreamReader(stream, Encoding.UTF8);
		_writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

		_ = ReceiveLoopAsync();
	}

	/// <inheritdoc/>
	public event Action<string>? MessageReceived;

	/// <inheritdoc/>
	public event Action? Disconnected;

	/// <inheritdoc/>
	public async Task SendAsync(string message)
	{
		await _writer.WriteLineAsync(message);
	}

	/// <inheritdoc/>
	public void Close()
	{
		Dispose();
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		_reader.Dispose();
		_writer.Dispose();
		_client.Dispose();
	}

	private async Task ReceiveLoopAsync()
	{
		try
		{
			while (true)
			{
				var line = await _reader.ReadLineAsync();
				if (line is null)
				{
					break;
				}

				MessageReceived?.Invoke(line);
			}
		}
		catch (IOException)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		finally
		{
			Disconnected?.Invoke();
		}
	}
}
