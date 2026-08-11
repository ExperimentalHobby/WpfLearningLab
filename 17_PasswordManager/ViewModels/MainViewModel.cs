using System.Collections.ObjectModel;
using System.Security.Cryptography;
using PasswordManager.Data;
using PasswordManager.Models;
using PasswordManager.Services;

namespace PasswordManager.ViewModels;

/// <summary>
/// パスワード管理アプリのメイン画面のViewModel。マスターパスワードによるロック解除とエントリのCRUDを担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	/// <summary>
	/// マスターパスワードの正誤判定に使う既知の平文。初回セットアップ時にこの文字列を暗号化して検証用値として保存する。
	/// </summary>
	private const string VerificationPlainText = "PASSWORD_MANAGER_VERIFY_V1";

	private readonly IPasswordEntryRepository _repository;
	private readonly IMasterKeyStore _masterKeyStore;
	private readonly IPasswordCryptoService _cryptoService;
	private readonly IClipboardService _clipboardService;

	private byte[]? _sessionKey;
	private bool _isUnlocked;
	private string _masterPasswordInput = string.Empty;
	private string _masterPasswordConfirmInput = string.Empty;
	private string _errorMessage = string.Empty;
	private string _inputSite = string.Empty;
	private string _inputUsername = string.Empty;
	private string _inputPassword = string.Empty;
	private PasswordEntryItem? _selectedEntry;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="repository">パスワードエントリの永続化を担うリポジトリ。</param>
	/// <param name="masterKeyStore">マスターキー設定の永続化を担うストア。</param>
	/// <param name="cryptoService">暗号化・鍵導出を担うサービス。</param>
	/// <param name="clipboardService">クリップボードコピーを担うサービス。</param>
	public MainViewModel(
		IPasswordEntryRepository repository,
		IMasterKeyStore masterKeyStore,
		IPasswordCryptoService cryptoService,
		IClipboardService clipboardService)
	{
		_repository = repository;
		_masterKeyStore = masterKeyStore;
		_cryptoService = cryptoService;
		_clipboardService = clipboardService;

		IsFirstRun = !_masterKeyStore.IsInitialized();

		UnlockCommand = new RelayCommand(Unlock, CanUnlock);
		AddCommand = new RelayCommand(Add, CanAddOrUpdate);
		UpdateCommand = new RelayCommand(Update, () => SelectedEntry is not null && CanAddOrUpdate());
		DeleteCommand = new RelayCommand(Delete, () => SelectedEntry is not null);
		CopyPasswordCommand = new RelayCommand<PasswordEntryItem>(CopyPassword);
	}

	/// <summary>マスターパスワードが未設定(初回起動)かどうか。</summary>
	public bool IsFirstRun { get; }

	/// <summary>ロックが解除され、一覧を操作できる状態かどうか。</summary>
	public bool IsUnlocked
	{
		get => _isUnlocked;
		private set => SetProperty(ref _isUnlocked, value);
	}

	/// <summary>マスターパスワード入力欄。</summary>
	public string MasterPasswordInput
	{
		get => _masterPasswordInput;
		set
		{
			if (SetProperty(ref _masterPasswordInput, value))
			{
				UnlockCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>マスターパスワード確認入力欄(初回のみ使用)。</summary>
	public string MasterPasswordConfirmInput
	{
		get => _masterPasswordConfirmInput;
		set
		{
			if (SetProperty(ref _masterPasswordConfirmInput, value))
			{
				UnlockCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>ロック解除失敗時などのエラーメッセージ。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>表示中のエントリ一覧。</summary>
	public ObservableCollection<PasswordEntryItem> Entries { get; } = [];

	/// <summary>入力フォーム: サイト名。</summary>
	public string InputSite
	{
		get => _inputSite;
		set
		{
			if (SetProperty(ref _inputSite, value))
			{
				RaiseAddUpdateCanExecuteChanged();
			}
		}
	}

	/// <summary>入力フォーム: ユーザー名。</summary>
	public string InputUsername
	{
		get => _inputUsername;
		set
		{
			if (SetProperty(ref _inputUsername, value))
			{
				RaiseAddUpdateCanExecuteChanged();
			}
		}
	}

	/// <summary>入力フォーム: パスワード。</summary>
	public string InputPassword
	{
		get => _inputPassword;
		set
		{
			if (SetProperty(ref _inputPassword, value))
			{
				RaiseAddUpdateCanExecuteChanged();
			}
		}
	}

	/// <summary>一覧で選択中のエントリ。選択すると入力フォームに内容が反映される。</summary>
	public PasswordEntryItem? SelectedEntry
	{
		get => _selectedEntry;
		set
		{
			if (SetProperty(ref _selectedEntry, value))
			{
				if (value is not null)
				{
					InputSite = value.Site;
					InputUsername = value.Username;
					InputPassword = value.Password;
				}

				UpdateCommand.RaiseCanExecuteChanged();
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>初回はマスターパスワードを新規設定し、2回目以降は入力内容でロックを解除するコマンド。</summary>
	public RelayCommand UnlockCommand { get; }

	/// <summary>入力フォームの内容を新規エントリとして追加するコマンド。</summary>
	public RelayCommand AddCommand { get; }

	/// <summary>選択中のエントリを入力フォームの内容で更新するコマンド。</summary>
	public RelayCommand UpdateCommand { get; }

	/// <summary>選択中のエントリを削除するコマンド。</summary>
	public RelayCommand DeleteCommand { get; }

	/// <summary>指定したエントリの復号済みパスワードをクリップボードにコピーするコマンド。</summary>
	public RelayCommand<PasswordEntryItem> CopyPasswordCommand { get; }

	private bool CanUnlock() =>
		IsFirstRun
			? !string.IsNullOrEmpty(MasterPasswordInput) && !string.IsNullOrEmpty(MasterPasswordConfirmInput)
			: !string.IsNullOrEmpty(MasterPasswordInput);

	private void Unlock()
	{
		ErrorMessage = string.Empty;

		if (IsFirstRun)
		{
			if (MasterPasswordInput != MasterPasswordConfirmInput)
			{
				ErrorMessage = "パスワードが一致しません。";
				return;
			}

			var salt = _cryptoService.GenerateSalt();
			var key = _cryptoService.DeriveKey(MasterPasswordInput, salt);
			var verificationValue = _cryptoService.Encrypt(VerificationPlainText, key);
			_masterKeyStore.Initialize(salt, verificationValue);

			_sessionKey = key;
			IsUnlocked = true;
			LoadEntries();
			return;
		}

		var existingSalt = _masterKeyStore.GetSalt();
		var derivedKey = _cryptoService.DeriveKey(MasterPasswordInput, existingSalt);

		if (!TryVerify(derivedKey))
		{
			ErrorMessage = "マスターパスワードが正しくありません。";
			return;
		}

		_sessionKey = derivedKey;
		IsUnlocked = true;
		LoadEntries();
	}

	private bool TryVerify(byte[] key)
	{
		try
		{
			return _cryptoService.Decrypt(_masterKeyStore.GetVerificationValue(), key) == VerificationPlainText;
		}
		catch (CryptographicException)
		{
			return false;
		}
		catch (FormatException)
		{
			return false;
		}
	}

	private void LoadEntries()
	{
		Entries.Clear();
		foreach (var entry in _repository.GetAll())
		{
			var password = _cryptoService.Decrypt(entry.EncryptedPassword, _sessionKey!);
			Entries.Add(new PasswordEntryItem(entry.Id, entry.Site, entry.Username, password));
		}
	}

	private bool CanAddOrUpdate() =>
		!string.IsNullOrWhiteSpace(InputSite) &&
		!string.IsNullOrWhiteSpace(InputUsername) &&
		!string.IsNullOrWhiteSpace(InputPassword);

	private void Add()
	{
		var entry = new PasswordEntry
		{
			Site = InputSite,
			Username = InputUsername,
			EncryptedPassword = _cryptoService.Encrypt(InputPassword, _sessionKey!),
		};
		_repository.Add(entry);
		Entries.Add(new PasswordEntryItem(entry.Id, InputSite, InputUsername, InputPassword));

		ClearInputs();
	}

	private void Update()
	{
		if (SelectedEntry is null)
		{
			return;
		}

		var entry = new PasswordEntry
		{
			Id = SelectedEntry.Id,
			Site = InputSite,
			Username = InputUsername,
			EncryptedPassword = _cryptoService.Encrypt(InputPassword, _sessionKey!),
		};
		_repository.Update(entry);

		SelectedEntry.Site = InputSite;
		SelectedEntry.Username = InputUsername;
		SelectedEntry.Password = InputPassword;
	}

	private void Delete()
	{
		if (SelectedEntry is null)
		{
			return;
		}

		_repository.Delete(SelectedEntry.Id);
		Entries.Remove(SelectedEntry);
		SelectedEntry = null;
		ClearInputs();
	}

	private void CopyPassword(PasswordEntryItem? entry)
	{
		if (entry is null)
		{
			return;
		}

		_clipboardService.SetText(entry.Password);
	}

	private void ClearInputs()
	{
		InputSite = string.Empty;
		InputUsername = string.Empty;
		InputPassword = string.Empty;
	}

	private void RaiseAddUpdateCanExecuteChanged()
	{
		AddCommand.RaiseCanExecuteChanged();
		UpdateCommand.RaiseCanExecuteChanged();
	}
}
