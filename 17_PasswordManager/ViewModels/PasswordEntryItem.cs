namespace PasswordManager.ViewModels;

/// <summary>
/// 一覧表示用のパスワードエントリラッパー。復号済みの平文パスワードと表示状態をセッション中のみ保持する。
/// </summary>
public class PasswordEntryItem : ObservableObject
{
	private string _site;
	private string _username;
	private string _password;
	private bool _isPasswordVisible;

	/// <summary>
	/// エントリを初期化する。
	/// </summary>
	/// <param name="id">エントリID。</param>
	/// <param name="site">サイト名。</param>
	/// <param name="username">ユーザー名。</param>
	/// <param name="password">復号済みの平文パスワード。</param>
	public PasswordEntryItem(int id, string site, string username, string password)
	{
		Id = id;
		_site = site;
		_username = username;
		_password = password;
	}

	/// <summary>エントリID。</summary>
	public int Id { get; }

	/// <summary>サイト名。</summary>
	public string Site
	{
		get => _site;
		set => SetProperty(ref _site, value);
	}

	/// <summary>ユーザー名。</summary>
	public string Username
	{
		get => _username;
		set => SetProperty(ref _username, value);
	}

	/// <summary>復号済みの平文パスワード。</summary>
	public string Password
	{
		get => _password;
		set => SetProperty(ref _password, value);
	}

	/// <summary>パスワードを平文表示中かどうか(一覧の表示/非表示切替用)。</summary>
	public bool IsPasswordVisible
	{
		get => _isPasswordVisible;
		set => SetProperty(ref _isPasswordVisible, value);
	}
}
