namespace ParallelImageProcessor.Models;

/// <summary>
/// バッチ処理の進捗状況。
/// </summary>
/// <param name="Completed">処理済み件数。</param>
/// <param name="Total">総件数。</param>
public record BatchProgress(int Completed, int Total);
