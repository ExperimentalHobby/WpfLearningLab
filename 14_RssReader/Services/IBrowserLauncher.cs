namespace RssReader.Services;

/// <summary>
/// URLを既定のブラウザで開く処理の抽象。
/// ViewModelのテストで実際にブラウザを起動せずに済むように分離する。
/// </summary>
public interface IBrowserLauncher
{
	/// <summary>
	/// 指定したURLを既定のブラウザで開く。
	/// </summary>
	void Open(string url);
}
