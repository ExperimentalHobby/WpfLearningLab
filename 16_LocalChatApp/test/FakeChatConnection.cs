using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実ネットワーク通信を行わない<see cref="IChatConnection"/>実装。
/// </summary>
public class FakeChatConnection : IChatConnection
{
	public List<string> SentMessages { get; } = [];
	public bool IsClosed { get; private set; }

	public event Action<string>? MessageReceived;
	public event Action? Disconnected;

	public Task SendAsync(string message)
	{
		SentMessages.Add(message);
		return Task.CompletedTask;
	}

	public void Close()
	{
		IsClosed = true;
	}

	public void RaiseMessageReceived(string message) => MessageReceived?.Invoke(message);

	public void RaiseDisconnected() => Disconnected?.Invoke();

	public void Dispose()
	{
	}
}
