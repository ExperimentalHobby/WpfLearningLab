using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ParallelImageProcessor.Models;

namespace ParallelImageProcessor.Services;

/// <summary>
/// <see cref="System.Drawing"/>(GDI+)を使い、画像のリサイズ/グレースケール化を単体・並列バッチの両方で実行する。
/// </summary>
public class ImageBatchProcessor : IImageBatchProcessor
{
	/// <inheritdoc/>
	public ImageProcessResult ProcessImage(string sourcePath, string destPath, ImageProcessingOptions options)
	{
		try
		{
			using var original = new Bitmap(sourcePath);
			using var resized = options.ResizeEnabled ? ResizeImage(original, options.TargetWidth, options.TargetHeight) : new Bitmap(original);
			using var final = options.GrayscaleEnabled ? ToGrayscale(resized) : resized;

			var destDir = Path.GetDirectoryName(destPath);
			if (!string.IsNullOrEmpty(destDir))
			{
				Directory.CreateDirectory(destDir);
			}
			final.Save(destPath, ImageFormat.Png);
			return new ImageProcessResult(sourcePath, Success: true, ErrorMessage: null);
		}
		// GDI+はファイル未存在・破損画像の読み込み時にIOException/ArgumentExceptionに加え、
		// 不正な画像データに対して(紛らわしいが)OutOfMemoryExceptionを投げることがある。
		catch (Exception ex) when (ex is IOException or ArgumentException or OutOfMemoryException or UnauthorizedAccessException)
		{
			return new ImageProcessResult(sourcePath, Success: false, ErrorMessage: ex.Message);
		}
	}

	/// <inheritdoc/>
	public async Task<BatchProcessResult> ProcessBatchAsync(
		IReadOnlyList<string> sourceFiles,
		string destinationFolder,
		ImageProcessingOptions options,
		IProgress<BatchProgress>? progress,
		CancellationToken cancellationToken)
	{
		Directory.CreateDirectory(destinationFolder);
		var stopwatch = Stopwatch.StartNew();
		var completed = 0;
		var successCount = 0;
		var failures = new ConcurrentBag<ImageProcessResult>();
		var total = sourceFiles.Count;

		var parallelOptions = new ParallelOptions
		{
			CancellationToken = cancellationToken,
			MaxDegreeOfParallelism = Environment.ProcessorCount,
		};

		await Parallel.ForEachAsync(sourceFiles, parallelOptions, (filePath, _) =>
		{
			var destPath = Path.Combine(destinationFolder, Path.GetFileName(filePath));
			var result = ProcessImage(filePath, destPath, options);
			if (result.Success)
			{
				Interlocked.Increment(ref successCount);
			}
			else
			{
				failures.Add(result);
			}

			var done = Interlocked.Increment(ref completed);
			progress?.Report(new BatchProgress(done, total));
			return ValueTask.CompletedTask;
		});

		stopwatch.Stop();
		return new BatchProcessResult(successCount, total - successCount, stopwatch.Elapsed, failures.ToList());
	}

	private static Bitmap ResizeImage(Bitmap source, int width, int height)
	{
		var result = new Bitmap(width, height);
		using var graphics = Graphics.FromImage(result);
		graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		graphics.DrawImage(source, 0, 0, width, height);
		return result;
	}

	private static Bitmap ToGrayscale(Bitmap source)
	{
		var result = new Bitmap(source.Width, source.Height);
		// NTSC加重平均によるグレースケール変換行列。R'=G'=B'=0.3R+0.59G+0.11B。
		var colorMatrix = new ColorMatrix(
		[
			[0.3f, 0.3f, 0.3f, 0, 0],
			[0.59f, 0.59f, 0.59f, 0, 0],
			[0.11f, 0.11f, 0.11f, 0, 0],
			[0, 0, 0, 1, 0],
			[0, 0, 0, 0, 1],
		]);
		using var attributes = new ImageAttributes();
		attributes.SetColorMatrix(colorMatrix);
		using var graphics = Graphics.FromImage(result);
		graphics.DrawImage(
			source,
			new Rectangle(0, 0, source.Width, source.Height),
			0, 0, source.Width, source.Height,
			GraphicsUnit.Pixel,
			attributes);
		return result;
	}
}
