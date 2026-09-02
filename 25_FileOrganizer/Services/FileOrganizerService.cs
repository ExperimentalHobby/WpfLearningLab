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

	private static OrganizeResult OrganizeFile(string filePath, string watchFolder, IReadOnlyList<SortingRule> rules)
	{
		var destinationFolderName = FileSortingRuleMatcher.GetDestinationFolder(filePath, rules);
		if (destinationFolderName is null)
		{
			return new OrganizeResult(filePath, null, false, DateTime.Now);
		}

		var destinationDir = Path.Combine(watchFolder, destinationFolderName);
		var destinationPath = Path.Combine(destinationDir, Path.GetFileName(filePath));

		try
		{
			Directory.CreateDirectory(destinationDir);
			File.Move(filePath, destinationPath);
			return new OrganizeResult(filePath, destinationPath, true, DateTime.Now);
		}
		catch (IOException ex)
		{
			return new OrganizeResult(filePath, destinationPath, false, DateTime.Now, ex.Message);
		}
	}
}
