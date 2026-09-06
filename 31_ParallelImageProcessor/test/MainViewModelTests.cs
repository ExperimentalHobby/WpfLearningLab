using System.Drawing;
using System.Drawing.Imaging;
using ParallelImageProcessor.Models;
using ParallelImageProcessor.Services;
using ParallelImageProcessor.Tests.Fakes;
using ParallelImageProcessor.ViewModels;

namespace ParallelImageProcessor.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。
/// フォルダ選択はフェイクに差し替えるが、画像処理自体は実の<see cref="ImageBatchProcessor"/>と
/// 一時フォルダの実ファイルで検証する。
/// </summary>
public class MainViewModelTests : IDisposable
{
	private readonly string _tempDir;

	public MainViewModelTests()
	{
		_tempDir = Path.Combine(Path.GetTempPath(), "ParallelImageProcessorVmTests_" + Guid.NewGuid());
		Directory.CreateDirectory(_tempDir);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDir))
		{
			Directory.Delete(_tempDir, recursive: true);
		}
	}

	private string CreateSourceFolderWithImages(int count)
	{
		var folder = Path.Combine(_tempDir, "source_" + Guid.NewGuid());
		Directory.CreateDirectory(folder);
		for (var i = 0; i < count; i++)
		{
			using var bitmap = new Bitmap(20, 10);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.Clear(Color.Red);
			}
			bitmap.Save(Path.Combine(folder, $"img{i}.png"), ImageFormat.Png);
		}
		return folder;
	}

	/// <summary>
	/// パス条件: フォルダ選択コマンドを実行すると、選択したフォルダがSourceFolder/DestinationFolderに反映されること。
	/// (SourceFolder/DestinationFolderのsetterがStartCommand(AsyncRelayCommand)を誤ってRelayCommandに
	/// キャストしInvalidCastExceptionで落ちていた実機バグの回帰テストを兼ねる)
	/// </summary>
	[Fact]
	public void SelectFolderCommand_実行するとフォルダパスが反映される()
	{
		var sourceFolder = CreateSourceFolderWithImages(1);
		var destFolder = Path.Combine(_tempDir, "dest");
		var picker = new FakeFolderPicker();
		var viewModel = new MainViewModel(new ImageBatchProcessor(), picker);

		picker.ResultPath = sourceFolder;
		viewModel.SelectSourceFolderCommand.Execute(null);
		picker.ResultPath = destFolder;
		viewModel.SelectDestinationFolderCommand.Execute(null);

		Assert.Equal(sourceFolder, viewModel.SourceFolder);
		Assert.Equal(destFolder, viewModel.DestinationFolder);
	}

	/// <summary>
	/// パス条件: 処理対象・保存先フォルダの両方が選択されるまでStartCommandが実行不可であること。
	/// </summary>
	[Fact]
	public void StartCommand_フォルダが両方選択されるまで実行不可()
	{
		var sourceFolder = CreateSourceFolderWithImages(1);
		var picker = new FakeFolderPicker();
		var viewModel = new MainViewModel(new ImageBatchProcessor(), picker);

		Assert.False(viewModel.StartCommand.CanExecute(null));

		picker.ResultPath = sourceFolder;
		viewModel.SelectSourceFolderCommand.Execute(null);
		Assert.False(viewModel.StartCommand.CanExecute(null));

		picker.ResultPath = Path.Combine(_tempDir, "dest");
		viewModel.SelectDestinationFolderCommand.Execute(null);
		Assert.True(viewModel.StartCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: StartCommandを実行すると画像が処理され、ResultSummaryTextに完了件数が表示されること。
	/// </summary>
	[Fact]
	public async Task StartCommand_実行すると画像が処理されサマリが表示される()
	{
		var sourceFolder = CreateSourceFolderWithImages(3);
		var destFolder = Path.Combine(_tempDir, "dest");
		var picker = new FakeFolderPicker();
		var viewModel = new MainViewModel(new ImageBatchProcessor(), picker) { ResizeEnabled = true, TargetWidth = 5, TargetHeight = 5 };
		picker.ResultPath = sourceFolder;
		viewModel.SelectSourceFolderCommand.Execute(null);
		picker.ResultPath = destFolder;
		viewModel.SelectDestinationFolderCommand.Execute(null);

		viewModel.StartCommand.Execute(null);
		// AsyncRelayCommand.Executeはasync voidのため、完了をポーリングで待つ。
		var waited = 0;
		while (viewModel.IsProcessing && waited < 5000)
		{
			await Task.Delay(50);
			waited += 50;
		}

		Assert.Contains("成功 3件", viewModel.ResultSummaryText);
		Assert.Equal(3, Directory.GetFiles(destFolder).Length);
	}

	/// <summary>
	/// パス条件: SourceFolderに存在しないフォルダを指定してStartCommandを実行しても、
	/// クラッシュせずResultSummaryTextにエラーメッセージが表示されること。
	/// </summary>
	[Fact]
	public async Task StartCommand_存在しないフォルダを指定してもクラッシュせずエラーが表示される()
	{
		var notExistFolder = Path.Combine(_tempDir, "not-exist-" + Guid.NewGuid());
		var destFolder = Path.Combine(_tempDir, "dest");
		var picker = new FakeFolderPicker { ResultPath = notExistFolder };
		var viewModel = new MainViewModel(new ImageBatchProcessor(), picker);
		viewModel.SelectSourceFolderCommand.Execute(null);
		picker.ResultPath = destFolder;
		viewModel.SelectDestinationFolderCommand.Execute(null);

		viewModel.StartCommand.Execute(null);
		var waited = 0;
		while (viewModel.IsProcessing && waited < 5000)
		{
			await Task.Delay(50);
			waited += 50;
		}

		Assert.False(string.IsNullOrEmpty(viewModel.ResultSummaryText));
	}

	/// <summary>
	/// パス条件: 並列処理により完了件数が逆順で報告されても、ProgressPercentageが後退しないこと。
	/// </summary>
	[Fact]
	public async Task StartCommand_進捗が逆順で報告されてもProgressPercentageが後退しない()
	{
		var sourceFolder = CreateSourceFolderWithImages(1);
		var destFolder = Path.Combine(_tempDir, "dest");
		var picker = new FakeFolderPicker();
		var processor = new FakeImageBatchProcessor
		{
			ProcessBatchAsyncImpl = (files, dest, options, progress, ct) =>
			{
				// 並列実行で完了順が入れ替わった状況を模擬する(3件目→1件目の順で報告)
				progress?.Report(new BatchProgress(3, 3));
				progress?.Report(new BatchProgress(1, 3));
				return Task.FromResult(new BatchProcessResult(3, 0, TimeSpan.Zero, []));
			},
		};
		var viewModel = new MainViewModel(processor, picker);
		picker.ResultPath = sourceFolder;
		viewModel.SelectSourceFolderCommand.Execute(null);
		picker.ResultPath = destFolder;
		viewModel.SelectDestinationFolderCommand.Execute(null);

		viewModel.StartCommand.Execute(null);
		var waited = 0;
		while (viewModel.IsProcessing && waited < 5000)
		{
			await Task.Delay(50);
			waited += 50;
		}

		Assert.Equal(100.0, viewModel.ProgressPercentage);
	}
}
