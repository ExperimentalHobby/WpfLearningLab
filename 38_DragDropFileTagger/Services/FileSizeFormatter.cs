namespace DragDropFileTagger.Services;

/// <summary>
/// バイト数を人間が読みやすい単位(B/KB/MB/GB)の文字列に整形する。
/// </summary>
public static class FileSizeFormatter
{
	private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB"];

	/// <summary>
	/// バイト数を整形する。
	/// </summary>
	public static string Format(long bytes)
	{
		double size = bytes;
		var unitIndex = 0;
		while (size >= 1024 && unitIndex < Units.Length - 1)
		{
			size /= 1024;
			unitIndex++;
		}
		return $"{size:0.##} {Units[unitIndex]}";
	}
}
