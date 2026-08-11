using System.Net;
using System.Net.Sockets;

namespace LocalChatApp.Services;

/// <summary>
/// <see cref="TcpListener"/>で1クライアントの接続を待ち受ける<see cref="IChatServer"/>実装。
/// </summary>
public class TcpChatServer : IChatServer
{
	/// <inheritdoc/>
	public async Task<IChatConnection> WaitForConnectionAsync(int port)
	{
		var listener = new TcpListener(IPAddress.Any, port);
		listener.Start();
		try
		{
			var client = await listener.AcceptTcpClientAsync();
			return new TcpChatConnection(client);
		}
		finally
		{
			listener.Stop();
		}
	}
}
