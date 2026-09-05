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
	/// <param name="url">開くURL。</param>
	/// <returns>開けた場合は true、安全でない/不正なURLとして拒否した場合は false。</returns>
	bool Open(string url);
}
