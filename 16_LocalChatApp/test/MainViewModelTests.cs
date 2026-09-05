using System.Net.Sockets;
using LocalChatApp.ViewModels;

namespace LocalChatApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakeChatServer? server = null,
		FakeChatClient? client = null,
		FakeUiDispatcher? dispatcher = null) =>
		new(server ?? new FakeChatServer(), client ?? new FakeChatClient(), dispatcher ?? new FakeUiDispatcher());

	/// <summary>
	/// パス条件: StartServerCommand実行で接続待機し、接続確立後IsConnectedがtrueになること
	/// </summary>
	[Fact]
	public async Task StartServerCommand_実行すると接続確立後IsConnectedがtrueになる()
	{
		var server = new FakeChatServer { ConnectionToReturn = new FakeChatConnection() };
		var viewModel = CreateViewModel(server);
		viewModel.Port = "5000";

		viewModel.StartServerCommand.Execute(null);
		await Task.Delay(50);

		Assert.True(viewModel.IsConnected);
	}

	/// <summary>
	/// パス条件: StartServerCommand実行時にポート使用中(SocketException)が発生しても、
	/// 例外を投げずErrorMessageが設定されること
	/// (StartServerCommandはAsyncRelayCommand経由のasync voidのため、ここで捕捉し損ねると
	/// 未処理例外でアプリ全体がクラッシュする)。
	/// </summary>
	[Fact]
	public async Task StartServerCommand_ポート使用中の場合ErrorMessageが設定される()
	{
		var server = new FakeChatServer { ExceptionToThrow = new SocketException((int)SocketError.AddressAlreadyInUse) };
		var viewModel = CreateViewModel(server);
		viewModel.Port = "5000";

		viewModel.StartServerCommand.Execute(null);
		await Task.Delay(50);

		Assert.False(viewModel.IsConnected);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: ConnectCommand実行でサーバーに接続し、IsConnectedがtrueになること
	/// </summary>
	[Fact]
	public async Task ConnectCommand_実行すると接続確立後IsConnectedがtrueになる()
	{
		var client = new FakeChatClient { ConnectionToReturn = new FakeChatConnection() };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";

		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);

		Assert.True(viewModel.IsConnected);
		Assert.Equal(("127.0.0.1", 5000), client.RequestedTarget);
	}

	/// <summary>
	/// パス条件: ConnectCommand実行時に接続失敗(SocketException)が発生しても、
	/// 例外を投げずErrorMessageが設定されること
	/// </summary>
	[Fact]
	public async Task ConnectCommand_接続失敗の場合ErrorMessageが設定される()
	{
		var client = new FakeChatClient { ExceptionToThrow = new SocketException((int)SocketError.ConnectionRefused) };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";

		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);

		Assert.False(viewModel.IsConnected);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: SendCommand実行でメッセージが送信され、自分のMessagesに追加されること
	/// </summary>
	[Fact]
	public async Task SendCommand_実行するとメッセージが送信されMessagesに追加される()
	{
		var connection = new FakeChatConnection();
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);
		viewModel.MessageInput = "こんにちは";

		viewModel.SendCommand.Execute(null);
		await Task.Delay(50);

		Assert.Contains("こんにちは", connection.SentMessages);
		Assert.Contains(viewModel.Messages, m => m.Text == "こんにちは");
	}

	/// <summary>
	/// パス条件: 切断後にSendCommandを実行し送信例外(IOException)が発生しても、
	/// 例外を投げずErrorMessageが設定されること
	/// </summary>
	[Fact]
	public async Task SendCommand_送信例外発生時ErrorMessageが設定される()
	{
		var connection = new FakeChatConnection { SendExceptionToThrow = new IOException("切断済み") };
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);
		viewModel.MessageInput = "こんにちは";

		viewModel.SendCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
		Assert.DoesNotContain(viewModel.Messages, m => m.Text == "こんにちは");
	}

	/// <summary>
	/// パス条件: 接続のMessageReceivedで相手のメッセージがMessagesに追加されること(IUiDispatcher経由)
	/// </summary>
	[Fact]
	public async Task MessageReceived_受信すると相手のメッセージがMessagesに追加される()
	{
		var connection = new FakeChatConnection();
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);

		connection.RaiseMessageReceived("やあ");

		Assert.Contains(viewModel.Messages, m => m.Text == "やあ" && m.Sender == Models.MessageSender.Remote);
	}

	/// <summary>
	/// パス条件: Disconnectedイベント発火でIsConnectedがfalseになり、システムメッセージが追加されること
	/// </summary>
	[Fact]
	public async Task Disconnected_発火するとIsConnectedがfalseになりシステムメッセージが追加される()
	{
		var connection = new FakeChatConnection();
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);

		connection.RaiseDisconnected();

		Assert.False(viewModel.IsConnected);
		Assert.Contains(viewModel.Messages, m => m.Sender == Models.MessageSender.System);
	}

	/// <summary>
	/// パス条件: Disconnectedイベント発火(相手都合の切断)後は、古い接続のMessageReceivedを
	/// 購読解除していること(再接続時に古い接続からの通知が混入しないようにするため)
	/// </summary>
	[Fact]
	public async Task Disconnected_発火後は古い接続のMessageReceived購読が解除される()
	{
		var connection = new FakeChatConnection();
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);
		connection.RaiseDisconnected();

		// 切断済みのはずの古い接続から(本来あり得ないが)メッセージが飛んできても、
		// 購読が解除されていればMessagesに追加されないはず。
		connection.RaiseMessageReceived("古い接続からのメッセージ");

		Assert.DoesNotContain(viewModel.Messages, m => m.Text == "古い接続からのメッセージ");
	}

	/// <summary>
	/// パス条件: DisconnectCommand実行で接続がCloseされ、IsConnectedがfalseになること
	/// </summary>
	[Fact]
	public async Task DisconnectCommand_実行すると接続がCloseされIsConnectedがfalseになる()
	{
		var connection = new FakeChatConnection();
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);

		viewModel.DisconnectCommand.Execute(null);

		Assert.True(connection.IsClosed);
		Assert.False(viewModel.IsConnected);
	}

	/// <summary>
	/// パス条件: MessageInputが空欄の場合、SendCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public async Task SendCommand_MessageInputが空欄の場合CanExecuteがfalseになる(string messageInput)
	{
		var connection = new FakeChatConnection();
		var client = new FakeChatClient { ConnectionToReturn = connection };
		var viewModel = CreateViewModel(client: client);
		viewModel.HostAddress = "127.0.0.1";
		viewModel.Port = "5000";
		viewModel.ConnectCommand.Execute(null);
		await Task.Delay(50);
		viewModel.MessageInput = messageInput;

		Assert.False(viewModel.SendCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 未接続の場合、SendCommandが実行不可になること
	/// </summary>
	[Fact]
	public void SendCommand_未接続の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.MessageInput = "こんにちは";

		Assert.False(viewModel.SendCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: Portが空欄/数値でない場合、StartServerCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("abc")]
	public void StartServerCommand_Portが不正な場合CanExecuteがfalseになる(string port)
	{
		var viewModel = CreateViewModel();
		viewModel.Port = port;

		Assert.False(viewModel.StartServerCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: HostAddressまたはPortが空欄の場合、ConnectCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("", "5000")]
	[InlineData("127.0.0.1", "")]
	[InlineData("127.0.0.1", "abc")]
	public void ConnectCommand_HostAddressまたはPortが不正な場合CanExecuteがfalseになる(string hostAddress, string port)
	{
		var viewModel = CreateViewModel();
		viewModel.HostAddress = hostAddress;
		viewModel.Port = port;

		Assert.False(viewModel.ConnectCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: Portを変更すると、StartServer/ConnectCommandのCanExecuteChangedが発火すること
	/// </summary>
	[Fact]
	public void Port_変更するとStartServerとConnectCommandのCanExecuteChangedが発火する()
	{
		var viewModel = CreateViewModel();
		var startServerRaised = false;
		var connectRaised = false;
		viewModel.StartServerCommand.CanExecuteChanged += (_, _) => startServerRaised = true;
		viewModel.ConnectCommand.CanExecuteChanged += (_, _) => connectRaised = true;

		viewModel.Port = "5000";

		Assert.True(startServerRaised);
		Assert.True(connectRaised);
	}

	/// <summary>
	/// パス条件: MessageInputを変更すると、SendCommandのCanExecuteChangedが発火すること
	/// </summary>
	[Fact]
	public void MessageInput_変更するとSendCommandのCanExecuteChangedが発火する()
	{
		var viewModel = CreateViewModel();
		var raised = false;
		viewModel.SendCommand.CanExecuteChanged += (_, _) => raised = true;

		viewModel.MessageInput = "こんにちは";

		Assert.True(raised);
	}
}
