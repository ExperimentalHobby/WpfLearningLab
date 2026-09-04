namespace MusicPlayer.Services;

/// <summary>
/// フォルダ内の音声ファイルを非同期に列挙する処理の抽象。
/// </summary>
public interface IAudioFileScanner
{
	/// <summary>
	/// 指定したフォルダ内の音声ファイル(mp3/wav/wma)のパスを非同期に取得する。
	/// </summary>
	/// <param name="folderPath">走査するフォルダのパス。</param>
	Task<IReadOnlyList<string>> GetAudioFilePathsAsync(string folderPath);
}
