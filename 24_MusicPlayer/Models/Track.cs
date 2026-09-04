namespace MusicPlayer.Models;

/// <summary>
/// プレイリスト上の1曲を表す。
/// </summary>
public class Track
{
	/// <summary>
	/// トラックを初期化する。
	/// </summary>
	/// <param name="filePath">音声ファイルのパス。</param>
	public Track(string filePath)
	{
		FilePath = filePath;
		Title = System.IO.Path.GetFileNameWithoutExtension(filePath);
	}

	/// <summary>音声ファイルのパス。</summary>
	public string FilePath { get; }

	/// <summary>曲名(ファイル名から拡張子を除いたもの)。</summary>
	public string Title { get; }
}
