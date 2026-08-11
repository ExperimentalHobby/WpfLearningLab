using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実際に待ち受けを行わない<see cref="IChatServer"/>実装。
/// </summary>
public class FakeChatServer : IChatServer
{
	public IChatConnection? ConnectionToReturn { get; set; }
	public Exception? ExceptionToThrow { get; set; }
	public int? RequestedPort { get; private set; }

	public Task<IChatConnection> WaitForConnectionAsync(int port)
	{
		RequestedPort = port;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(ConnectionToReturn!);
	}
}
