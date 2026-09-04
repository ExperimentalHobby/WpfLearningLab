namespace FileOrganizer.Services;

/// <summary>
/// フォルダ直下へのファイル作成を監視する処理の抽象。
/// </summary>
public interface IDirectoryWatcher : IDisposable
{
	/// <summary>
	/// 監視対象フォルダ直下に新しいファイルが作成されたときに発生する。
	/// イベント引数は作成されたファイルのフルパス。
	/// </summary>
	event Action<string>? FileCreated;

	/// <summary>
	/// 指定したフォルダの監視を開始する。
	/// </summary>
	/// <param name="folderPath">監視対象フォルダのパス。</param>
	void Start(string folderPath);

	/// <summary>
	/// 監視を停止する。
	/// </summary>
	void Stop();
}
