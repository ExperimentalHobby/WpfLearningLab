namespace ParallelImageProcessor.Models;

/// <summary>
/// 1枚の画像に対する処理結果。
/// </summary>
/// <param name="SourcePath">処理対象の元画像パス。</param>
/// <param name="Success">処理が成功したかどうか。</param>
/// <param name="ErrorMessage">失敗した場合のエラーメッセージ。成功時は<see langword="null"/>。</param>
public record ImageProcessResult(string SourcePath, bool Success, string? ErrorMessage);
