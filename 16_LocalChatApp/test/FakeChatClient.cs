using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="LocalChatApp.ViewModels.MainViewModel"/> のテスト用に、実際に接続を行わない<see cref="IChatClient"/>実装。
/// </summary>
public class FakeChatClient : IChatClient
{
	/// <summary><see cref="ConnectAsync"/>が返す値(テスト用)。</summary>
	public IChatConnection? ConnectionToReturn { get; set; }

	/// <summary>設定すると<see cref="ConnectAsync"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <summary>直近の<see cref="ConnectAsync"/>呼び出し引数(テスト用)。</summary>
	public (string Host, int Port)? RequestedTarget { get; private set; }

	/// <inheritdoc/>
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
