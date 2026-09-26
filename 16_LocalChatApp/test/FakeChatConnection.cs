using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="LocalChatApp.ViewModels.MainViewModel"/> のテスト用に、実ネットワーク通信を行わない<see cref="IChatConnection"/>実装。
/// </summary>
public class FakeChatConnection : IChatConnection
{
	/// <summary><see cref="SendAsync"/>で送信されたメッセージの履歴(テスト用)。</summary>
	public List<string> SentMessages { get; } = [];

	/// <summary><see cref="Close"/>が呼ばれたかどうか(テスト用)。</summary>
	public bool IsClosed { get; private set; }

	/// <summary>設定すると<see cref="SendAsync"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? SendExceptionToThrow { get; set; }

	/// <inheritdoc/>
	public event Action<string>? MessageReceived;

	/// <inheritdoc/>
	public event Action? Disconnected;

	/// <inheritdoc/>
	public Task SendAsync(string message)
	{
		if (SendExceptionToThrow is not null)
		{
			throw SendExceptionToThrow;
		}

		SentMessages.Add(message);
		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	public void Close()
	{
		IsClosed = true;
	}

	/// <summary><see cref="MessageReceived"/>イベントを発火する(テスト用)。</summary>
	/// <param name="message">受信メッセージとしてイベント引数に渡す文字列。</param>
	public void RaiseMessageReceived(string message) => MessageReceived?.Invoke(message);

	/// <summary><see cref="Disconnected"/>イベントを発火する(テスト用)。</summary>
	public void RaiseDisconnected() => Disconnected?.Invoke();

	/// <inheritdoc/>
	public void Dispose()
	{
	}
}
