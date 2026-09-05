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

	/// <summary>クリップボードにコピーしたパスワードを自動クリアするまでの時間。</summary>
	private static readonly TimeSpan ClipboardAutoClearDelay = TimeSpan.FromSeconds(30);

	private readonly IPasswordEntryRepository _repository;
	private readonly IMasterKeyStore _masterKeyStore;
	private readonly IPasswordCryptoService _cryptoService;
	private readonly IClipboardService _clipboardService;
	private readonly IDelayedActionScheduler _clipboardClearScheduler;

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
	/// <param name="clipboardClearScheduler">
	/// クリップボード自動クリアの遅延実行を担うスケジューラ。省略時は実時間で動作する既定の実装を使う。
	/// </param>
	public MainViewModel(
		IPasswordEntryRepository repository,
		IMasterKeyStore masterKeyStore,
		IPasswordCryptoService cryptoService,
		IClipboardService clipboardService,
		IDelayedActionScheduler? clipboardClearScheduler = null)
	{
		_repository = repository;
		_masterKeyStore = masterKeyStore;
		_cryptoService = cryptoService;
		_clipboardService = clipboardService;
		_clipboardClearScheduler = clipboardClearScheduler ?? new TimerDelayedActionScheduler();

		IsFirstRun = !_masterKeyStore.IsInitialized();

		UnlockCommand = new RelayCommand(Unlock, CanUnlock);
		LockCommand = new RelayCommand(Lock, () => IsUnlocked);
		AddCommand = new RelayCommand(Add, CanAddOrUpdate);
		UpdateCommand = new RelayCommand(Update, () => SelectedEntry is not null && CanAddOrUpdate());
		DeleteCommand = new RelayCommand(Delete, () => SelectedEntry is not null);
		CopyPasswordCommand = new RelayCommand<PasswordEntryItem>(CopyPassword);
	}

	private bool _isFirstRun;

	/// <summary>
	/// マスターパスワードが未設定(初回起動)かどうか。初回セットアップが完了すると
	/// falseになり、以後LockCommandで再ロックしても(初回セットアップ画面には戻らず)
	/// 常に既存のマスターパスワードでの解除を求めるようになる。
	/// </summary>
	public bool IsFirstRun
	{
		get => _isFirstRun;
		private set => SetProperty(ref _isFirstRun, value);
	}

	/// <summary>ロックが解除され、一覧を操作できる状態かどうか。</summary>
	public bool IsUnlocked
	{
		get => _isUnlocked;
		private set
		{
			if (SetProperty(ref _isUnlocked, value))
			{
				LockCommand.RaiseCanExecuteChanged();
			}
		}
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

	/// <summary>
	/// 再度ロックするコマンド。セッションキーをゼロクリアし、復号済みのエントリ一覧を破棄する。
	/// </summary>
	public RelayCommand LockCommand { get; }

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
			IsFirstRun = false;
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

	/// <summary>
	/// 再度ロックし、セッションキーと復号済みエントリ一覧を破棄する。
	/// </summary>
	/// <remarks>
	/// セッションキー(<see cref="_sessionKey"/>、byte[])は<see cref="Array.Clear(Array)"/>で
	/// 内容を確実にゼロクリアしてから破棄する。一方、復号済みパスワードを保持する
	/// <see cref="PasswordEntryItem.Password"/>はC#の文字列(不変)であるため、byte[]と同様に
	/// メモリ上の内容を確実にゼロクリアすることはできない。真に安全な実装には
	/// <see cref="System.Security.SecureString"/>または<c>char[]</c>ベースへの全面的な設計変更
	/// (XAMLバインディング・PasswordBox連携を含む)が必要であり、今回は見送る。
	/// <see cref="Entries"/>をクリアして参照を切ることで、GC対象にする(露出時間を短縮する)
	/// という限定的な緩和にとどめる。
	/// </remarks>
	private void Lock()
	{
		if (_sessionKey is not null)
		{
			Array.Clear(_sessionKey, 0, _sessionKey.Length);
			_sessionKey = null;
		}

		Entries.Clear();
		SelectedEntry = null;
		ClearInputs();
		MasterPasswordInput = string.Empty;
		MasterPasswordConfirmInput = string.Empty;
		ErrorMessage = string.Empty;
		IsUnlocked = false;
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

	/// <summary>復号に失敗したエントリに表示するプレースホルダ文字列。</summary>
	private const string DecryptionFailedPlaceholder = "*** 復号できませんでした ***";

	private void LoadEntries()
	{
		Entries.Clear();
		foreach (var entry in _repository.GetAll())
		{
			string password;
			try
			{
				password = _cryptoService.Decrypt(entry.EncryptedPassword, _sessionKey!);
			}
			catch (Exception ex) when (ex is CryptographicException or FormatException)
			{
				// DB破損、または別のマスターパスワード(=別の鍵)で暗号化されたデータが
				// 混入していると復号に失敗する。1件の破損が原因でロック解除自体が
				// クラッシュしてはならないため、その1件だけプレースホルダ表示にして
				// 他の正常なエントリの表示は続ける(StickyNotesの1件スキップと同様の方針)。
				password = DecryptionFailedPlaceholder;
			}

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

		ClearInputs();
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

		var password = entry.Password;
		_clipboardService.SetText(password);

		// コピーした平文パスワードがクリップボードに残り続けないよう、一定時間後に
		// 自動クリアする。ただしその間に別の内容がコピーされていたら上書きしない
		// (ClearIfUnchangedがコピー時の内容と一致する場合のみクリアする)。
		_clipboardClearScheduler.Schedule(ClipboardAutoClearDelay, () => _clipboardService.ClearIfUnchanged(password));
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
