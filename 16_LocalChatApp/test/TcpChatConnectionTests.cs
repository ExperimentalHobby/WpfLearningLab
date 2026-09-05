using System.Net;
using System.Net.Sockets;
using LocalChatApp.Services;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="TcpChatConnection"/> の単体テスト。
/// 実際にループバックアドレス(127.0.0.1、OSが割り当てる空きポート)で2つの<see cref="TcpClient"/>を接続させて検証する。
/// </summary>
public class TcpChatConnectionTests : IAsyncLifetime
{
	private TcpListener _listener = null!;
	private TcpClient _clientSideClient = null!;
	private TcpClient _serverSideClient = null!;
	private TcpChatConnection _clientSideConnection = null!;
	private TcpChatConnection _serverSideConnection = null!;

	public async Task InitializeAsync()
	{
		_listener = new TcpListener(IPAddress.Loopback, 0);
		_listener.Start();
		var port = ((IPEndPoint)_listener.LocalEndpoint).Port;

		_clientSideClient = new TcpClient();
		var connectTask = _clientSideClient.ConnectAsync(IPAddress.Loopback, port);
		_serverSideClient = await _listener.AcceptTcpClientAsync();
		await connectTask;

		_clientSideConnection = new TcpChatConnection(_clientSideClient);
		_serverSideConnection = new TcpChatConnection(_serverSideClient);
	}

	public Task DisposeAsync()
	{
		_clientSideConnection.Dispose();
		_serverSideConnection.Dispose();
		_listener.Stop();
		return Task.CompletedTask;
	}

	/// <summary>
	/// パス条件: 送信したメッセージが相手側でMessageReceivedとして受信できること
	/// </summary>
	[Fact]
	public async Task SendAsync_送信したメッセージが相手側でMessageReceivedとして受信できる()
	{
		var tcs = new TaskCompletionSource<string>();
		_serverSideConnection.MessageReceived += message => tcs.TrySetResult(message);

		await _clientSideConnection.SendAsync("こんにちは");
		var received = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

		Assert.Equal("こんにちは", received);
	}

	/// <summary>
	/// パス条件: 相手が接続を閉じると、こちら側でDisconnectedイベントが発火すること
	/// </summary>
	[Fact]
	public async Task Disconnected_相手が接続を閉じるとイベントが発火する()
	{
		var tcs = new TaskCompletionSource();
		_serverSideConnection.Disconnected += () => tcs.TrySetResult();

		_clientSideConnection.Close();
		await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

		Assert.True(tcs.Task.IsCompletedSuccessfully);
	}

	/// <summary>
	/// パス条件: Disposeを呼んでも例外を投げないこと。
	/// readerとwriterは同一のNetworkStreamをラップしているため、reader→writerの順に
	/// Disposeすると、writerのDispose時のFlushが(readerのDisposeで)既に閉じられた
	/// ストリームに対して行われ例外になり得る。
	/// </summary>
	[Fact]
	public void Dispose_呼んでも例外を投げない()
	{
		var exception = Record.Exception(() => _clientSideConnection.Dispose());

		Assert.Null(exception);
	}
}
