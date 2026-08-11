using System.Collections.ObjectModel;
using LocalChatApp.Models;
using LocalChatApp.Services;

namespace LocalChatApp.ViewModels;

/// <summary>
/// 簡易チャットアプリのメイン画面のViewModel。サーバー待受/クライアント接続・メッセージ送受信を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IChatServer _server;
	private readonly IChatClient _client;
	private readonly IUiDispatcher _dispatcher;

	private IChatConnection? _connection;

	private string _port = string.Empty;
	private string _hostAddress = string.Empty;
	private string _messageInput = string.Empty;
	private bool _isConnected;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel(IChatServer server, IChatClient client, IUiDispatcher dispatcher)
	{
		_server = server;
		_client = client;
		_dispatcher = dispatcher;
		StartServerCommand = new AsyncRelayCommand(StartServerAsync, CanStartServer);
		ConnectCommand = new AsyncRelayCommand(ConnectAsync, CanConnect);
		SendCommand = new AsyncRelayCommand(SendAsync, CanSend);
		DisconnectCommand = new RelayCommand(Disconnect);
	}

	/// <summary>サーバー待受・クライアント接続先のポート番号(文字列入力)。</summary>
	public string Port
	{
		get => _port;
		set
		{
			if (SetProperty(ref _port, value))
			{
				StartServerCommand.RaiseCanExecuteChanged();
				ConnectCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>クライアント接続先ホストアドレス。</summary>
	public string HostAddress
	{
		get => _hostAddress;
		set
		{
			if (SetProperty(ref _hostAddress, value))
			{
				ConnectCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>送信するメッセージの入力欄。</summary>
	public string MessageInput
	{
		get => _messageInput;
		set
		{
			if (SetProperty(ref _messageInput, value))
			{
				SendCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>接続中かどうか。</summary>
	public bool IsConnected
	{
		get => _isConnected;
		private set
		{
			if (SetProperty(ref _isConnected, value))
			{
				SendCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>チャット欄に表示するメッセージ一覧。</summary>
	public ObservableCollection<ChatMessage> Messages { get; } = [];

	/// <summary>
	/// サーバーとしてクライアントの接続を待ち受けるコマンド。
	/// </summary>
	public AsyncRelayCommand StartServerCommand { get; }

	/// <summary>
	/// クライアントとしてサーバーに接続するコマンド。
	/// </summary>
	public AsyncRelayCommand ConnectCommand { get; }

	/// <summary>
	/// <see cref="MessageInput"/>の内容を相手に送信するコマンド。
	/// </summary>
	public AsyncRelayCommand SendCommand { get; }

	/// <summary>
	/// 現在の接続を切断するコマンド。
	/// </summary>
	public RelayCommand DisconnectCommand { get; }

	private static bool IsValidPort(string port) => int.TryParse(port, out var value) && value is > 0 and <= 65535;

	private bool CanStartServer() => IsValidPort(Port);

	private bool CanConnect() => !string.IsNullOrWhiteSpace(HostAddress) && IsValidPort(Port);

	private async Task StartServerAsync()
	{
		var port = int.Parse(Port);
		var connection = await _server.WaitForConnectionAsync(port);
		AttachConnection(connection);
	}

	private async Task ConnectAsync()
	{
		var port = int.Parse(Port);
		var connection = await _client.ConnectAsync(HostAddress, port);
		AttachConnection(connection);
	}

	private void AttachConnection(IChatConnection connection)
	{
		_connection = connection;
		_connection.MessageReceived += OnMessageReceived;
		_connection.Disconnected += OnDisconnected;
		IsConnected = true;
	}

	private void OnMessageReceived(string text)
	{
		_dispatcher.Invoke(() =>
		{
			Messages.Add(new ChatMessage { Sender = MessageSender.Remote, Text = text, Timestamp = DateTime.Now });
		});
	}

	private void OnDisconnected()
	{
		_dispatcher.Invoke(() =>
		{
			IsConnected = false;
			Messages.Add(new ChatMessage { Sender = MessageSender.System, Text = "接続が切断されました。", Timestamp = DateTime.Now });
		});
	}

	private void Disconnect()
	{
		if (_connection is null)
		{
			return;
		}

		_connection.MessageReceived -= OnMessageReceived;
		_connection.Disconnected -= OnDisconnected;
		_connection.Close();
		_connection = null;
		IsConnected = false;
		Messages.Add(new ChatMessage { Sender = MessageSender.System, Text = "接続を切断しました。", Timestamp = DateTime.Now });
	}

	private bool CanSend() => IsConnected && !string.IsNullOrWhiteSpace(MessageInput);

	private async Task SendAsync()
	{
		if (_connection is null)
		{
			return;
		}

		var text = MessageInput;
		await _connection.SendAsync(text);
		Messages.Add(new ChatMessage { Sender = MessageSender.Local, Text = text, Timestamp = DateTime.Now });
		MessageInput = string.Empty;
	}
}
