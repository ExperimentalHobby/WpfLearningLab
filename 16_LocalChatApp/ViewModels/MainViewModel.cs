using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
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
	private string _errorMessage = string.Empty;

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

	/// <summary>エラーメッセージ。エラーが無い場合は空文字。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

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
		ErrorMessage = string.Empty;
		try
		{
			var port = int.Parse(Port);
			var connection = await _server.WaitForConnectionAsync(port);
			AttachConnection(connection);
		}
		catch (SocketException ex)
		{
			// ポート使用中等の接続エラーは、ネットワークアプリでは常態的に起こり得る。
			// StartServerCommand(AsyncRelayCommand)のExecuteはasync void実装でcatchを
			// 持たないため、ここで捕捉し損ねると未処理例外でアプリ全体がクラッシュする。
			ErrorMessage = $"待受を開始できませんでした。\n{ex.Message}";
		}
	}

	private async Task ConnectAsync()
	{
		ErrorMessage = string.Empty;
		try
		{
			var port = int.Parse(Port);
			var connection = await _client.ConnectAsync(HostAddress, port);
			AttachConnection(connection);
		}
		catch (SocketException ex)
		{
			// 接続拒否・名前解決失敗等の接続エラーへの対応。理由はStartServerAsyncと同様。
			ErrorMessage = $"接続できませんでした。\n{ex.Message}";
		}
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
			// 相手都合の切断でも、手動のDisconnect()と同様にイベント購読を解除し
			// _connectionをnullにする。これを怠ると、再接続時に古い(切断済みの)
			// 接続への購読が残り続けてしまう。
			DetachConnection();
			IsConnected = false;
			Messages.Add(new ChatMessage { Sender = MessageSender.System, Text = "接続が切断されました。", Timestamp = DateTime.Now });
		});
	}

	private void DetachConnection()
	{
		if (_connection is null)
		{
			return;
		}

		_connection.MessageReceived -= OnMessageReceived;
		_connection.Disconnected -= OnDisconnected;
		_connection = null;
	}

	private void Disconnect()
	{
		if (_connection is null)
		{
			return;
		}

		_connection.Close();
		DetachConnection();
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
		try
		{
			await _connection.SendAsync(text);
		}
		catch (Exception ex) when (ex is IOException or ObjectDisposedException)
		{
			// 切断後の送信等で発生し得る。SendCommand(AsyncRelayCommand)のExecuteは
			// async void実装でcatchを持たないため、ここで捕捉し損ねると未処理例外で
			// アプリ全体がクラッシュする。
			ErrorMessage = "送信に失敗しました。接続が切断されている可能性があります。";
			return;
		}

		Messages.Add(new ChatMessage { Sender = MessageSender.Local, Text = text, Timestamp = DateTime.Now });
		MessageInput = string.Empty;
	}
}
