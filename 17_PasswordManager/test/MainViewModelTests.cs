using PasswordManager.Services;
using PasswordManager.ViewModels;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// フェイクのリポジトリ・マスターキーストア・クリップボードと、実際の<see cref="AesPasswordCryptoService"/>で検証する。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakePasswordEntryRepository? repository = null,
		FakeMasterKeyStore? masterKeyStore = null,
		FakeClipboardService? clipboardService = null,
		FakeDelayedActionScheduler? scheduler = null) =>
		new(
			repository ?? new FakePasswordEntryRepository(),
			masterKeyStore ?? new FakeMasterKeyStore(),
			new AesPasswordCryptoService(),
			clipboardService ?? new FakeClipboardService(),
			scheduler ?? new FakeDelayedActionScheduler());

	/// <summary>
	/// パス条件: マスターキーが未初期化の場合、IsFirstRunがtrueになること
	/// </summary>
	[Fact]
	public void コンストラクタ_未初期化の場合IsFirstRunがtrueになる()
	{
		var viewModel = CreateViewModel();

		Assert.True(viewModel.IsFirstRun);
	}

	/// <summary>
	/// パス条件: マスターキーが初期化済みの場合、IsFirstRunがfalseになること
	/// </summary>
	[Fact]
	public void コンストラクタ_初期化済みの場合IsFirstRunがfalseになる()
	{
		var masterKeyStore = new FakeMasterKeyStore();
		masterKeyStore.Initialize([1, 2, 3], "dummy");

		var viewModel = CreateViewModel(masterKeyStore: masterKeyStore);

		Assert.False(viewModel.IsFirstRun);
	}

	/// <summary>
	/// パス条件: 初回セットアップでパスワードと確認用パスワードが一致する場合、ロックが解除されること
	/// </summary>
	[Fact]
	public void UnlockCommand_初回セットアップでパスワードが一致すると解除される()
	{
		var viewModel = CreateViewModel();
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";

		viewModel.UnlockCommand.Execute(null);

		Assert.True(viewModel.IsUnlocked);
	}

	/// <summary>
	/// パス条件: 初回セットアップでパスワードと確認用パスワードが一致しない場合、ロックされたままエラーが表示されること
	/// </summary>
	[Fact]
	public void UnlockCommand_初回セットアップでパスワードが不一致だとロックのままエラーが表示される()
	{
		var viewModel = CreateViewModel();
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "different-password";

		viewModel.UnlockCommand.Execute(null);

		Assert.False(viewModel.IsUnlocked);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 初期化済みの環境で正しいマスターパスワードを入力するとロックが解除され、一覧が復号されて表示されること
	/// </summary>
	[Fact]
	public void UnlockCommand_既存環境で正しいパスワードだと解除され一覧が復号表示される()
	{
		var repository = new FakePasswordEntryRepository();
		var masterKeyStore = new FakeMasterKeyStore();
		var crypto = new AesPasswordCryptoService();
		var salt = crypto.GenerateSalt();
		var key = crypto.DeriveKey("master-password", salt);
		masterKeyStore.Initialize(salt, crypto.Encrypt("PASSWORD_MANAGER_VERIFY_V1", key));
		repository.Add(new Models.PasswordEntry
		{
			Site = "example.com",
			Username = "user1",
			EncryptedPassword = crypto.Encrypt("P@ssw0rd!", key),
		});
		var viewModel = CreateViewModel(repository, masterKeyStore);
		viewModel.MasterPasswordInput = "master-password";

		viewModel.UnlockCommand.Execute(null);

		Assert.True(viewModel.IsUnlocked);
		Assert.Single(viewModel.Entries);
		Assert.Equal("example.com", viewModel.Entries[0].Site);
		Assert.Equal("P@ssw0rd!", viewModel.Entries[0].Password);
	}

	/// <summary>
	/// パス条件: 1件だけ復号できない(別の鍵で暗号化された、または破損した)エントリが
	/// 混ざっていても、例外を投げずロック解除でき、他の正常なエントリは復号表示され、
	/// 復号できなかったエントリはプレースホルダ表示になること
	/// (1件の破損データが原因で、ロック解除自体がクラッシュしてはならないため)
	/// </summary>
	[Fact]
	public void UnlockCommand_復号できないエントリが混在してもクラッシュせず他は表示される()
	{
		var repository = new FakePasswordEntryRepository();
		var masterKeyStore = new FakeMasterKeyStore();
		var crypto = new AesPasswordCryptoService();
		var salt = crypto.GenerateSalt();
		var key = crypto.DeriveKey("master-password", salt);
		masterKeyStore.Initialize(salt, crypto.Encrypt("PASSWORD_MANAGER_VERIFY_V1", key));
		repository.Add(new Models.PasswordEntry
		{
			Site = "good.example.com",
			Username = "user1",
			EncryptedPassword = crypto.Encrypt("P@ssw0rd!", key),
		});
		// 別の鍵で暗号化された(=正しい鍵では復号できない)壊れたエントリを混入させる。
		var otherKey = crypto.DeriveKey("different-password", crypto.GenerateSalt());
		repository.Add(new Models.PasswordEntry
		{
			Site = "broken.example.com",
			Username = "user2",
			EncryptedPassword = crypto.Encrypt("Secret!", otherKey),
		});
		var viewModel = CreateViewModel(repository, masterKeyStore);
		viewModel.MasterPasswordInput = "master-password";

		var exception = Record.Exception(() => viewModel.UnlockCommand.Execute(null));

		Assert.Null(exception);
		Assert.True(viewModel.IsUnlocked);
		Assert.Equal(2, viewModel.Entries.Count);
		Assert.Equal("P@ssw0rd!", viewModel.Entries.Single(e => e.Site == "good.example.com").Password);
		Assert.NotEqual("Secret!", viewModel.Entries.Single(e => e.Site == "broken.example.com").Password);
	}

	/// <summary>
	/// パス条件: 初期化済みの環境で誤ったマスターパスワードを入力するとロックのままエラーが表示されること
	/// </summary>
	[Fact]
	public void UnlockCommand_既存環境で誤ったパスワードだとロックのままエラーが表示される()
	{
		var masterKeyStore = new FakeMasterKeyStore();
		var crypto = new AesPasswordCryptoService();
		var salt = crypto.GenerateSalt();
		var key = crypto.DeriveKey("master-password", salt);
		masterKeyStore.Initialize(salt, crypto.Encrypt("PASSWORD_MANAGER_VERIFY_V1", key));
		var viewModel = CreateViewModel(masterKeyStore: masterKeyStore);
		viewModel.MasterPasswordInput = "wrong-password";

		viewModel.UnlockCommand.Execute(null);

		Assert.False(viewModel.IsUnlocked);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: AddCommand実行で入力内容が暗号化されて保存され、一覧に追加されること
	/// </summary>
	[Fact]
	public void AddCommand_入力内容が保存され一覧に追加される()
	{
		var repository = new FakePasswordEntryRepository();
		var viewModel = CreateViewModel(repository);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);

		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);

		Assert.Single(viewModel.Entries);
		Assert.Equal("example.com", viewModel.Entries[0].Site);
		Assert.Equal("P@ssw0rd!", viewModel.Entries[0].Password);
		Assert.Single(repository.GetAll());
		Assert.NotEqual("P@ssw0rd!", repository.GetAll()[0].EncryptedPassword);
	}

	/// <summary>
	/// パス条件: Site/Username/Passwordのいずれかが空欄の場合、AddCommandのCanExecuteがfalseになること
	/// </summary>
	[Theory]
	[InlineData("", "user1", "P@ssw0rd!")]
	[InlineData("example.com", "", "P@ssw0rd!")]
	[InlineData("example.com", "user1", "")]
	public void AddCommand_入力欄が空だとCanExecuteがfalseになる(string site, string username, string password)
	{
		var viewModel = CreateViewModel();
		viewModel.InputSite = site;
		viewModel.InputUsername = username;
		viewModel.InputPassword = password;

		Assert.False(viewModel.AddCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: UpdateCommand実行で選択中エントリの内容が更新されること(パスワードも再暗号化される)
	/// </summary>
	[Fact]
	public void UpdateCommand_選択中エントリの内容が更新される()
	{
		var repository = new FakePasswordEntryRepository();
		var viewModel = CreateViewModel(repository);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);
		viewModel.SelectedEntry = viewModel.Entries[0];

		viewModel.InputUsername = "user2";
		viewModel.InputPassword = "NewP@ssw0rd!";
		viewModel.UpdateCommand.Execute(null);

		Assert.Equal("user2", viewModel.Entries[0].Username);
		Assert.Equal("NewP@ssw0rd!", viewModel.Entries[0].Password);
		var stored = repository.GetAll()[0];
		Assert.Equal("user2", stored.Username);
	}

	/// <summary>
	/// パス条件: UpdateCommand実行後、入力欄がクリアされること(AddCommand実行後と対称的な挙動にする)
	/// </summary>
	[Fact]
	public void UpdateCommand_実行後に入力欄がクリアされる()
	{
		var viewModel = CreateViewModel();
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);
		viewModel.SelectedEntry = viewModel.Entries[0];
		viewModel.InputUsername = "user2";
		viewModel.InputPassword = "NewP@ssw0rd!";

		viewModel.UpdateCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.InputSite);
		Assert.Equal(string.Empty, viewModel.InputUsername);
		Assert.Equal(string.Empty, viewModel.InputPassword);
	}

	/// <summary>
	/// パス条件: DeleteCommand実行で選択中エントリがリポジトリと一覧の両方から削除されること
	/// </summary>
	[Fact]
	public void DeleteCommand_選択中エントリが削除される()
	{
		var repository = new FakePasswordEntryRepository();
		var viewModel = CreateViewModel(repository);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);
		viewModel.SelectedEntry = viewModel.Entries[0];

		viewModel.DeleteCommand.Execute(null);

		Assert.Empty(viewModel.Entries);
		Assert.Empty(repository.GetAll());
	}

	/// <summary>
	/// パス条件: エントリが未選択の場合、UpdateCommand/DeleteCommandのCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void UpdateDeleteCommand_未選択の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();

		Assert.False(viewModel.UpdateCommand.CanExecute(null));
		Assert.False(viewModel.DeleteCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: CopyPasswordCommand実行で対象エントリの復号済みパスワードがクリップボードにコピーされること
	/// </summary>
	[Fact]
	public void CopyPasswordCommand_対象エントリのパスワードがクリップボードにコピーされる()
	{
		var clipboardService = new FakeClipboardService();
		var viewModel = CreateViewModel(clipboardService: clipboardService);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);

		viewModel.CopyPasswordCommand.Execute(viewModel.Entries[0]);

		Assert.Equal("P@ssw0rd!", clipboardService.CopiedText);
	}

	/// <summary>
	/// パス条件: CopyPasswordCommand実行時に、一定時間後クリップボードを自動クリアする
	/// 処理がスケジュールされ、その時間が経過するとクリップボードがクリアされること
	/// (平文パスワードがクリップボードに残り続けるのを防ぐため)
	/// </summary>
	[Fact]
	public void CopyPasswordCommand_実行後一定時間でクリップボードが自動クリアされる()
	{
		var clipboardService = new FakeClipboardService();
		var scheduler = new FakeDelayedActionScheduler();
		var viewModel = CreateViewModel(clipboardService: clipboardService, scheduler: scheduler);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);

		viewModel.CopyPasswordCommand.Execute(viewModel.Entries[0]);
		Assert.Equal("P@ssw0rd!", clipboardService.CopiedText);
		Assert.False(clipboardService.WasCleared);

		scheduler.RunAll();

		Assert.True(clipboardService.WasCleared);
		Assert.Null(clipboardService.CopiedText);
	}

	/// <summary>
	/// パス条件: クリップボード自動クリアが実行される前に、ユーザーが別の内容を
	/// クリップボードにコピーしていた場合は、それを上書きしてクリアしないこと
	/// </summary>
	[Fact]
	public void CopyPasswordCommand_自動クリア前に別の内容がコピーされていたら上書きしない()
	{
		var clipboardService = new FakeClipboardService();
		var scheduler = new FakeDelayedActionScheduler();
		var viewModel = CreateViewModel(clipboardService: clipboardService, scheduler: scheduler);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);
		viewModel.CopyPasswordCommand.Execute(viewModel.Entries[0]);

		clipboardService.SetText("別の内容");
		scheduler.RunAll();

		Assert.Equal("別の内容", clipboardService.CopiedText);
	}

	/// <summary>
	/// パス条件: エントリを選択するとPasswordVisibleの切り替えができ、対象エントリのIsPasswordVisibleが反転すること
	/// </summary>
	[Fact]
	public void IsPasswordVisible_トグルするとエントリの表示状態が反転する()
	{
		var viewModel = CreateViewModel();
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);
		var entry = viewModel.Entries[0];

		entry.IsPasswordVisible = true;

		Assert.True(entry.IsPasswordVisible);
	}

	/// <summary>
	/// パス条件: LockCommand実行でIsUnlockedがfalseに戻り、復号済みエントリ一覧
	/// (平文パスワードを含む)がクリアされること(再ロック機能)
	/// </summary>
	[Fact]
	public void LockCommand_実行するとIsUnlockedがfalseに戻りEntriesがクリアされる()
	{
		var viewModel = CreateViewModel();
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.InputSite = "example.com";
		viewModel.InputUsername = "user1";
		viewModel.InputPassword = "P@ssw0rd!";
		viewModel.AddCommand.Execute(null);

		viewModel.LockCommand.Execute(null);

		Assert.False(viewModel.IsUnlocked);
		Assert.Empty(viewModel.Entries);
	}

	/// <summary>
	/// パス条件: LockCommand実行後、再度正しいマスターパスワードでUnlockCommandを実行すると
	/// 再度ロック解除できること(セッションキーのクリア後も再ロック解除に支障がないこと)
	/// </summary>
	[Fact]
	public void LockCommand_実行後に再度正しいパスワードでUnlockCommandを実行すると解除できる()
	{
		var repository = new FakePasswordEntryRepository();
		var masterKeyStore = new FakeMasterKeyStore();
		var viewModel = CreateViewModel(repository, masterKeyStore);
		viewModel.MasterPasswordInput = "master-password";
		viewModel.MasterPasswordConfirmInput = "master-password";
		viewModel.UnlockCommand.Execute(null);
		viewModel.LockCommand.Execute(null);

		viewModel.MasterPasswordInput = "master-password";
		viewModel.UnlockCommand.Execute(null);

		Assert.True(viewModel.IsUnlocked);
	}
}
