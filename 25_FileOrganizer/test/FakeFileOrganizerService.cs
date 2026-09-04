using FileOrganizer.Models;
using FileOrganizer.Services;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のファイル移動を行わない<see cref="IFileOrganizerService"/>実装。
/// </summary>
public class FakeFileOrganizerService : IFileOrganizerService
{
	/// <summary>単一ファイルの振り分け要求で返す結果を作る関数(未設定時は移動なしの結果を返す)。</summary>
	public Func<string, string, IReadOnlyList<SortingRule>, OrganizeResult>? OrganizeFileResultFactory { get; set; }

	/// <summary>一括振り分け要求で返す結果一覧。</summary>
	public IReadOnlyList<OrganizeResult> ExistingFilesResult { get; set; } = [];

	/// <summary>最後に一括振り分けが要求された監視フォルダのパス。</summary>
	public string? RequestedWatchFolder { get; private set; }

	/// <inheritdoc/>
	public Task<OrganizeResult> OrganizeFileAsync(string filePath, string watchFolder, IReadOnlyList<SortingRule> rules)
	{
		var result = OrganizeFileResultFactory?.Invoke(filePath, watchFolder, rules)
			?? new OrganizeResult(filePath, null, false, DateTime.Now);
		return Task.FromResult(result);
	}

	/// <inheritdoc/>
	public Task<IReadOnlyList<OrganizeResult>> OrganizeExistingFilesAsync(string watchFolder, IReadOnlyList<SortingRule> rules)
	{
		RequestedWatchFolder = watchFolder;
		return Task.FromResult(ExistingFilesResult);
	}
}
