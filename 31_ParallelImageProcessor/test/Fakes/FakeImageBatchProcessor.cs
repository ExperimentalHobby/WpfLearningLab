using ParallelImageProcessor.Models;
using ParallelImageProcessor.Services;

namespace ParallelImageProcessor.Tests.Fakes;

/// <summary>
/// 実際の画像処理を行わない<see cref="IImageBatchProcessor"/>のフェイク。
/// <see cref="ProcessBatchAsyncImpl"/>を差し替えることで、進捗報告のタイミング・順序を模擬できる。
/// </summary>
internal class FakeImageBatchProcessor : IImageBatchProcessor
{
	public Func<IReadOnlyList<string>, string, ImageProcessingOptions, IProgress<BatchProgress>?, CancellationToken, Task<BatchProcessResult>>?
		ProcessBatchAsyncImpl { get; set; }

	public ImageProcessResult ProcessImage(string sourcePath, string destPath, ImageProcessingOptions options) =>
		new(sourcePath, Success: true, ErrorMessage: null);

	public Task<BatchProcessResult> ProcessBatchAsync(
		IReadOnlyList<string> sourceFiles,
		string destinationFolder,
		ImageProcessingOptions options,
		IProgress<BatchProgress>? progress,
		CancellationToken cancellationToken)
	{
		if (ProcessBatchAsyncImpl is not null)
		{
			return ProcessBatchAsyncImpl(sourceFiles, destinationFolder, options, progress, cancellationToken);
		}

		return Task.FromResult(new BatchProcessResult(sourceFiles.Count, 0, TimeSpan.Zero, []));
	}
}
