using ParallelImageProcessor.Models;

namespace ParallelImageProcessor.Services;

/// <summary>
/// 画像のリサイズ/グレースケール化を単体・並列バッチの両方で実行するサービスの抽象。
/// </summary>
public interface IImageBatchProcessor
{
	/// <summary>
	/// 1枚の画像を処理し、<paramref name="destPath"/>に保存する。
	/// </summary>
	ImageProcessResult ProcessImage(string sourcePath, string destPath, ImageProcessingOptions options);

	/// <summary>
	/// 複数の画像を並列処理する。進捗は<paramref name="progress"/>に報告され、
	/// <paramref name="cancellationToken"/>により中断できる。
	/// </summary>
	Task<BatchProcessResult> ProcessBatchAsync(
		IReadOnlyList<string> sourceFiles,
		string destinationFolder,
		ImageProcessingOptions options,
		IProgress<BatchProgress>? progress,
		CancellationToken cancellationToken);
}
