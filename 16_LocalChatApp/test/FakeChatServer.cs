using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="LocalChatApp.ViewModels.MainViewModel"/> のテスト用に、実際に待ち受けを行わない<see cref="IChatServer"/>実装。
/// </summary>
public class FakeChatServer : IChatServer
{
	/// <summary><see cref="WaitForConnectionAsync"/>が返す値(テスト用)。</summary>
	public IChatConnection? ConnectionToReturn { get; set; }

	/// <summary>設定すると<see cref="WaitForConnectionAsync"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <summary>直近の<see cref="WaitForConnectionAsync"/>呼び出し引数(テスト用)。</summary>
	public int? RequestedPort { get; private set; }

	/// <inheritdoc/>
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
