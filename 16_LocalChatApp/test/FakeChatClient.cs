using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実際に接続を行わない<see cref="IChatClient"/>実装。
/// </summary>
public class FakeChatClient : IChatClient
{
	public IChatConnection? ConnectionToReturn { get; set; }
	public Exception? ExceptionToThrow { get; set; }
	public (string Host, int Port)? RequestedTarget { get; private set; }

	public Task<IChatConnection> ConnectAsync(string host, int port)
	{
		RequestedTarget = (host, port);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(ConnectionToReturn!);
	}
}
