using System.Net.Sockets;

namespace LocalChatApp.Services;

/// <summary>
/// <see cref="TcpClient"/>でサーバーに接続する<see cref="IChatClient"/>実装。
/// </summary>
public class TcpChatClient : IChatClient
{
	/// <inheritdoc/>
	public async Task<IChatConnection> ConnectAsync(string host, int port)
	{
		var client = new TcpClient();
		await client.ConnectAsync(host, port);
		return new TcpChatConnection(client);
	}
}
