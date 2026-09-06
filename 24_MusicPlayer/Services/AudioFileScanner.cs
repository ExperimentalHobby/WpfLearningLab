using System.IO;

namespace MusicPlayer.Services;

/// <summary>
/// ファイルシステムから音声ファイルを非同期に列挙する<see cref="IAudioFileScanner"/>実装。
/// </summary>
public class AudioFileScanner : IAudioFileScanner
{
	private static readonly string[] AudioExtensions = [".mp3", ".wav", ".wma"];

	/// <inheritdoc/>
	public Task<IReadOnlyList<string>> GetAudioFilePathsAsync(string folderPath) =>
		Task.Run(() => ScanDirectory(folderPath));

	private static IReadOnlyList<string> ScanDirectory(string folderPath)
	{
		if (!Directory.Exists(folderPath))
		{
			return [];
		}

		try
		{
			return Directory.EnumerateFiles(folderPath)
				.Where(path => AudioExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
				.OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
				.ToList();
		}
		catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
		{
			// アクセス権限がないフォルダ等では、存在しないフォルダと同様に空の一覧を返す。
			return [];
		}
	}
}
