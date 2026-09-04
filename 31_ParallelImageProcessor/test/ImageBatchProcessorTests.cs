using System.Drawing;
using System.Drawing.Imaging;
using ParallelImageProcessor.Models;
using ParallelImageProcessor.Services;

namespace ParallelImageProcessor.Tests;

/// <summary>
/// <see cref="ImageBatchProcessor"/>のテスト。
/// モックを使わず、一時フォルダに実際の画像ファイルを生成してファイルI/Oごと検証する。
/// </summary>
public class ImageBatchProcessorTests : IDisposable
{
	private readonly string _tempDir;

	public ImageBatchProcessorTests()
	{
		_tempDir = Path.Combine(Path.GetTempPath(), "ParallelImageProcessorTests_" + Guid.NewGuid());
		Directory.CreateDirectory(_tempDir);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDir))
		{
			Directory.Delete(_tempDir, recursive: true);
		}
	}

	private string CreateSampleImage(string fileName, int width = 20, int height = 10, Color? color = null)
	{
		var path = Path.Combine(_tempDir, fileName);
		using var bitmap = new Bitmap(width, height);
		using (var graphics = Graphics.FromImage(bitmap))
		{
			graphics.Clear(color ?? Color.FromArgb(255, 200, 50, 10));
		}
		bitmap.Save(path, ImageFormat.Png);
		return path;
	}

	/// <summary>
	/// パス条件: リサイズを有効にして処理すると、出力画像が指定した幅・高さになること。
	/// </summary>
	[Fact]
	public void ProcessImage_リサイズ有効の場合出力画像が指定サイズになる()
	{
		var sourcePath = CreateSampleImage("source.png", width: 40, height: 20);
		var destPath = Path.Combine(_tempDir, "out.png");
		var options = new ImageProcessingOptions(ResizeEnabled: true, TargetWidth: 10, TargetHeight: 5, GrayscaleEnabled: false);
		var sut = new ImageBatchProcessor();

		var result = sut.ProcessImage(sourcePath, destPath, options);

		Assert.True(result.Success);
		using var output = new Bitmap(destPath);
		Assert.Equal(10, output.Width);
		Assert.Equal(5, output.Height);
	}

	/// <summary>
	/// パス条件: グレースケールを有効にして処理すると、出力画像の各ピクセルでR・G・Bの値が一致すること。
	/// </summary>
	[Fact]
	public void ProcessImage_グレースケール有効の場合出力画像の各ピクセルでRGBが一致する()
	{
		var sourcePath = CreateSampleImage("color.png", color: Color.FromArgb(255, 200, 50, 10));
		var destPath = Path.Combine(_tempDir, "gray.png");
		var options = new ImageProcessingOptions(ResizeEnabled: false, TargetWidth: 0, TargetHeight: 0, GrayscaleEnabled: true);
		var sut = new ImageBatchProcessor();

		var result = sut.ProcessImage(sourcePath, destPath, options);

		Assert.True(result.Success);
		using var output = new Bitmap(destPath);
		var pixel = output.GetPixel(0, 0);
		Assert.Equal(pixel.R, pixel.G);
		Assert.Equal(pixel.G, pixel.B);
	}

	/// <summary>
	/// パス条件: 存在しないファイルを処理しようとすると、Successがfalseになりエラーメッセージが設定されること。
	/// </summary>
	[Fact]
	public void ProcessImage_存在しないファイルの場合Successがfalseでエラーメッセージが設定される()
	{
		var sourcePath = Path.Combine(_tempDir, "not-exist.png");
		var destPath = Path.Combine(_tempDir, "out.png");
		var options = new ImageProcessingOptions(false, 0, 0, false);
		var sut = new ImageBatchProcessor();

		var result = sut.ProcessImage(sourcePath, destPath, options);

		Assert.False(result.Success);
		Assert.NotNull(result.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 複数画像をバッチ処理すると、成功件数が処理対象件数と一致すること。
	/// </summary>
	[Fact]
	public async Task ProcessBatchAsync_複数画像を処理しSuccessCountが件数と一致する()
	{
		var sourceFiles = Enumerable.Range(0, 5).Select(i => CreateSampleImage($"img{i}.png")).ToList();
		var destFolder = Path.Combine(_tempDir, "dest");
		Directory.CreateDirectory(destFolder);
		var options = new ImageProcessingOptions(true, 8, 8, true);
		var sut = new ImageBatchProcessor();

		var result = await sut.ProcessBatchAsync(sourceFiles, destFolder, options, progress: null, CancellationToken.None);

		Assert.Equal(5, result.SuccessCount);
		Assert.Equal(0, result.FailureCount);
		Assert.Equal(5, Directory.GetFiles(destFolder).Length);
	}

	/// <summary>
	/// パス条件: バッチ処理中、IProgressに処理済み件数が総件数まで報告されること。
	/// </summary>
	[Fact]
	public async Task ProcessBatchAsync_進捗がIProgressで報告される()
	{
		var sourceFiles = Enumerable.Range(0, 4).Select(i => CreateSampleImage($"img{i}.png")).ToList();
		var destFolder = Path.Combine(_tempDir, "dest");
		Directory.CreateDirectory(destFolder);
		var options = new ImageProcessingOptions(false, 0, 0, false);
		var sut = new ImageBatchProcessor();
		var reports = new List<BatchProgress>();
		var progress = new Progress<BatchProgress>(p => { lock (reports) { reports.Add(p); } });

		await sut.ProcessBatchAsync(sourceFiles, destFolder, options, progress, CancellationToken.None);
		// Progress<T>はSynchronizationContext経由でポストされるため、テストスレッド(SynchronizationContextなし)では
		// 実行と同期的に呼ばれる。念のため短時間待って全件反映を待つ。
		await Task.Delay(50);

		Assert.Contains(reports, r => r.Completed == 4 && r.Total == 4);
	}

	/// <summary>
	/// パス条件: 処理中にキャンセルすると、全件処理が完了する前に中断されること。
	/// </summary>
	[Fact]
	public async Task ProcessBatchAsync_キャンセルすると全件処理される前に中断される()
	{
		var sourceFiles = Enumerable.Range(0, 50).Select(i => CreateSampleImage($"img{i}.png")).ToList();
		var destFolder = Path.Combine(_tempDir, "dest");
		Directory.CreateDirectory(destFolder);
		var options = new ImageProcessingOptions(true, 4, 4, true);
		var sut = new ImageBatchProcessor();
		using var cts = new CancellationTokenSource();
		var progress = new Progress<BatchProgress>(p =>
		{
			if (p.Completed >= 1)
			{
				cts.Cancel();
			}
		});

		// Parallel.ForEachAsyncは内部でTaskCanceledException(OperationCanceledExceptionのサブクラス)を
		// スローするため、派生型も許容するThrowsAnyAsyncで検証する。
		await Assert.ThrowsAnyAsync<OperationCanceledException>(
			() => sut.ProcessBatchAsync(sourceFiles, destFolder, options, progress, cts.Token));

		Assert.True(Directory.GetFiles(destFolder).Length < 50);
	}
}
