namespace FileOrganizer.Models;

/// <summary>
/// 1ファイル分の振り分け結果。処理ログの表示に使う。
/// </summary>
/// <param name="SourcePath">振り分け対象だった元のファイルパス。</param>
/// <param name="DestinationPath">移動先のファイルパス。移動しなかった場合は<see langword="null"/>。</param>
/// <param name="Moved">実際に移動できたかどうか。</param>
/// <param name="Timestamp">処理日時。</param>
/// <param name="ErrorMessage">移動に失敗した場合のエラーメッセージ。</param>
public record OrganizeResult(string SourcePath, string? DestinationPath, bool Moved, DateTime Timestamp, string? ErrorMessage = null);
