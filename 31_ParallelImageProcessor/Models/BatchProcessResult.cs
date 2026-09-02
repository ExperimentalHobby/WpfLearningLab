namespace ParallelImageProcessor.Models;

/// <summary>
/// バッチ処理全体の結果サマリ。
/// </summary>
/// <param name="SuccessCount">成功件数。</param>
/// <param name="FailureCount">失敗件数。</param>
/// <param name="Elapsed">処理に要した時間。</param>
/// <param name="Failures">失敗した処理結果の一覧。</param>
public record BatchProcessResult(int SuccessCount, int FailureCount, TimeSpan Elapsed, IReadOnlyList<ImageProcessResult> Failures);
