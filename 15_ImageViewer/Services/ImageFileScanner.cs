using System.IO;

namespace ImageViewer.Services;

/// <summary>
/// ファイルシステムから画像ファイルを列挙する<see cref="IImageFileScanner"/>実装。
/// </summary>
public class ImageFileScanner : IImageFileScanner
{
	private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif"];

	/// <inheritdoc/>
	public IReadOnlyList<string> GetImageFilePaths(string folderPath)
	{
		if (!Directory.Exists(folderPath))
		{
			return [];
		}

		return Directory.EnumerateFiles(folderPath)
			.Where(path => ImageExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
			.OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
			.ToList();
	}
}
