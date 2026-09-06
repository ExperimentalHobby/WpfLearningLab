using System.IO;
using FileOrganizer.Models;

namespace FileOrganizer.Services;

/// <summary>
/// 実際のファイルシステム(<see cref="File.Move(string, string)"/>)を使う<see cref="IFileOrganizerService"/>実装。
/// </summary>
public class FileOrganizerService : IFileOrganizerService
{
	/// <inheritdoc/>
	public Task<OrganizeResult> OrganizeFileAsync(string filePath, string watchFolder, IReadOnlyList<SortingRule> rules) =>
		Task.Run(() => OrganizeFile(filePath, watchFolder, rules));

	/// <inheritdoc/>
	public async Task<IReadOnlyList<OrganizeResult>> OrganizeExistingFilesAsync(string watchFolder, IReadOnlyList<SortingRule> rules)
	{
		var results = new List<OrganizeResult>();
		foreach (var filePath in Directory.EnumerateFiles(watchFolder))
		{
			results.Add(await OrganizeFileAsync(filePath, watchFolder, rules));
		}

		return results;
	}

	private const int MaxMoveAttempts = 4;
	private static readonly TimeSpan MoveRetryDelay = TimeSpan.FromMilliseconds(100);

	private static OrganizeResult OrganizeFile(string filePath, string watchFolder, IReadOnlyList<SortingRule> rules)
	{
		var destinationFolderName = FileSortingRuleMatcher.GetDestinationFolder(filePath, rules);
		if (destinationFolderName is null)
		{
			return new OrganizeResult(filePath, null, false, DateTime.Now);
		}

		var destinationDir = Path.Combine(watchFolder, destinationFolderName);
		var destinationPath = Path.Combine(destinationDir, Path.GetFileName(filePath));

		for (var attempt = 1; attempt <= MaxMoveAttempts; attempt++)
		{
			try
			{
				Directory.CreateDirectory(destinationDir);
				File.Move(filePath, destinationPath);
				return new OrganizeResult(filePath, destinationPath, true, DateTime.Now);
			}
			catch (IOException) when (attempt < MaxMoveAttempts)
			{
				// FileSystemWatcher.Createdはファイルの書き込みが完了する前(コピー中で
				// ロックされている状態)でも発火することがある。少し待って再試行する。
				Thread.Sleep(MoveRetryDelay);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				return new OrganizeResult(filePath, destinationPath, false, DateTime.Now, ex.Message);
			}
		}

		return new OrganizeResult(filePath, destinationPath, false, DateTime.Now, "リトライ上限に達したため移動できませんでした。");
	}
}
