using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using ClaudeChatClient.Models;
using ClaudeChatClient.Services;

namespace ClaudeChatClient.ViewModels;

/// <summary>
/// Claude APIチャットクライアントのメイン画面ViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private const string VerificationPlainText = "CLAUDE_CHAT_CLIENT_VERIFICATION_V1";

	private readonly IApiKeyStore _apiKeyStore;
	private readonly IApiKeyCryptoService _cryptoService;
	private readonly Func<string, IClaudeApiClient> _claudeApiClientFactory;

	private IClaudeApiClient? _claudeApiClient;
	private CancellationTokenSource? _sendCts;

	private bool _isUnlocked;
	private string _masterPasswordInput = string.Empty;
	private string _apiKeyInput = string.Empty;
	private string _inputText = string.Empty;
	private string _errorMessage = string.Empty;
	private bool _isSending;

	/// <summary>
	/// APIキーが未保存(初回起動)かどうか。
	/// </summary>
	public bool IsFirstRun { get; }

	/// <summary>
	/// ロックが解除され、チャット操作が可能かどうか。
	/// </summary>
	public bool IsUnlocked
	{
		get => _isUnlocked;
		private set => SetProperty(ref _isUnlocked, value);
	}

	/// <summary>
	/// マスターパスワード入力欄の値。
	/// </summary>
	public string MasterPasswordInput
	{
		get => _masterPasswordInput;
		set => SetProperty(ref _masterPasswordInput, value);
	}

	/// <summary>
	/// (初回セットアップ時のみ使う)APIキー入力欄の値。
	/// </summary>
	public string ApiKeyInput
	{
		get => _apiKeyInput;
		set => SetProperty(ref _apiKeyInput, value);
	}

	/// <summary>
	/// メッセージ送信欄の入力値。
	/// </summary>
	public string InputText
	{
		get => _inputText;
		set => SetProperty(ref _inputText, value);
	}

	/// <summary>
	/// 直近のエラーメッセージ。
	/// </summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>
	/// 送信中(ストリーミング受信中)かどうか。
	/// </summary>
	public bool IsSending
	{
		get => _isSending;
		private set => SetProperty(ref _isSending, value);
	}

	/// <summary>
	/// 会話履歴。
	/// </summary>
	public ObservableCollection<ChatMessage> Messages { get; } = [];

	/// <summary>
	/// 初回セットアップ(マスターパスワード新規設定+APIキー保存)を行うコマンド。
	/// </summary>
	public ICommand SetupCommand { get; }

	/// <summary>
	/// マスターパスワードでロックを解除するコマンド。
	/// </summary>
	public ICommand UnlockCommand { get; }

	/// <summary>
	/// メッセージを送信するコマンド。
	/// </summary>
	public ICommand SendCommand { get; }

	/// <summary>
	/// 送信中のリクエストをキャンセルするコマンド。
	/// </summary>
	public ICommand CancelCommand { get; }

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	/// <param name="apiKeyStore">APIキーの永続化先。</param>
	/// <param name="cryptoService">APIキーの暗号化・復号サービス。</param>
	/// <param name="claudeApiClientFactory">復号済みAPIキーから<see cref="IClaudeApiClient"/>を生成するファクトリ。</param>
	public MainViewModel(
		IApiKeyStore apiKeyStore,
		IApiKeyCryptoService cryptoService,
		Func<string, IClaudeApiClient> claudeApiClientFactory)
	{
		_apiKeyStore = apiKeyStore;
		_cryptoService = cryptoService;
		_claudeApiClientFactory = claudeApiClientFactory;

		IsFirstRun = !_apiKeyStore.TryLoad(out _);

		SetupCommand = new RelayCommand(
			Setup, () => !string.IsNullOrWhiteSpace(MasterPasswordInput) && !string.IsNullOrWhiteSpace(ApiKeyInput));
		UnlockCommand = new RelayCommand(Unlock, () => !string.IsNullOrWhiteSpace(MasterPasswordInput));
		SendCommand = new RelayCommand(
			() => _ = SendAsync(), () => IsUnlocked && !IsSending && !string.IsNullOrWhiteSpace(InputText));
		CancelCommand = new RelayCommand(Cancel, () => IsSending);
	}

	private void Setup()
	{
		var salt = _cryptoService.GenerateSalt();
		var key = _cryptoService.DeriveKey(MasterPasswordInput, salt);
		var verification = _cryptoService.Encrypt(VerificationPlainText, key);
		var encryptedApiKey = _cryptoService.Encrypt(ApiKeyInput, key);

		_apiKeyStore.Save(new ApiKeyRecord(salt, verification, encryptedApiKey));
		_claudeApiClient = _claudeApiClientFactory(ApiKeyInput);
		IsUnlocked = true;
		ErrorMessage = string.Empty;
	}

	private void Unlock()
	{
		if (!_apiKeyStore.TryLoad(out var record) || record is null)
		{
			ErrorMessage = "保存されたAPIキーが見つかりません。初回セットアップを行ってください。";
			return;
		}

		var key = _cryptoService.DeriveKey(MasterPasswordInput, record.Salt);

		try
		{
			var verification = _cryptoService.Decrypt(record.VerificationCipherText, key);
			if (verification != VerificationPlainText)
			{
				ErrorMessage = "マスターパスワードが正しくありません。";
				return;
			}

			var apiKey = _cryptoService.Decrypt(record.EncryptedApiKey, key);
			_claudeApiClient = _claudeApiClientFactory(apiKey);
			IsUnlocked = true;
			ErrorMessage = string.Empty;
		}
		catch (CryptographicException)
		{
			ErrorMessage = "マスターパスワードが正しくありません。";
		}
	}

	/// <summary>
	/// 現在の入力内容を送信し、ストリーミング応答を逐次<see cref="Messages"/>に反映する。
	/// </summary>
	public async Task SendAsync()
	{
		if (_claudeApiClient is null || string.IsNullOrWhiteSpace(InputText))
		{
			return;
		}

		var userMessage = new ChatMessage(ChatRole.User, InputText);
		Messages.Add(userMessage);
		InputText = string.Empty;

		var assistantIndex = Messages.Count;
		Messages.Add(new ChatMessage(ChatRole.Assistant, string.Empty));

		// APIへは直前までの確定した発言のみを送る(末尾の空のAssistantメッセージは含めない)。
		var history = Messages.Take(assistantIndex).ToList();

		_sendCts = new CancellationTokenSource();
		IsSending = true;
		ErrorMessage = string.Empty;

		try
		{
			var builder = new StringBuilder();
			await foreach (var chunk in _claudeApiClient.StreamMessageAsync(history, _sendCts.Token))
			{
				builder.Append(chunk);
				Messages[assistantIndex] = new ChatMessage(ChatRole.Assistant, builder.ToString());
			}
		}
		catch (OperationCanceledException)
		{
			ErrorMessage = "送信をキャンセルしました。";
		}
		catch (ClaudeApiException ex)
		{
			ErrorMessage = ex.Message;
		}
		finally
		{
			IsSending = false;
			_sendCts = null;
		}
	}

	private void Cancel() => _sendCts?.Cancel();
}
