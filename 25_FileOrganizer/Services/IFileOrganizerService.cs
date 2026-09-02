using FileOrganizer.Models;

namespace FileOrganizer.Services;

/// <summary>
/// ファイルを振り分けルールに従って移動する処理の抽象。
/// </summary>
public interface IFileOrganizerService
{
	/// <summary>
	/// 指定した1ファイルを振り分けルールに従って移動する。
	/// 一致するルールが無い場合は移動せず、<see cref="OrganizeResult.Moved"/>が<see langword="false"/>の結果を返す。
	/// </summary>
	/// <param name="filePath">対象ファイルのパス。</param>
	/// <param name="watchFolder">監視フォルダのパス(移動先はこの配下に作成される)。</param>
	/// <param name="rules">振り分けルール一覧。</param>
	Task<OrganizeResult> OrganizeFileAsync(string filePath, string watchFolder, IReadOnlyList<SortingRule> rules);

	/// <summary>
	/// 監視フォルダ直下(サブフォルダを除く)の既存ファイルを一括で振り分ける。
	/// </summary>
	/// <param name="watchFolder">監視フォルダのパス。</param>
	/// <param name="rules">振り分けルール一覧。</param>
	Task<IReadOnlyList<OrganizeResult>> OrganizeExistingFilesAsync(string watchFolder, IReadOnlyList<SortingRule> rules);
}
