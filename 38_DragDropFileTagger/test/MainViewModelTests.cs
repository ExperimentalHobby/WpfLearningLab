using DragDropFileTagger.ViewModels;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。ファイル取り込みは実の一時ファイルを使って検証する。
/// </summary>
public class MainViewModelTests : IDisposable
{
	private readonly string _tempDir;

	public MainViewModelTests()
	{
		_tempDir = Path.Combine(Path.GetTempPath(), "DragDropFileTaggerVmTests_" + Guid.NewGuid());
		Directory.CreateDirectory(_tempDir);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDir))
		{
			Directory.Delete(_tempDir, recursive: true);
		}
	}

	private string CreateSampleFile(string name, string content = "test")
	{
		var path = Path.Combine(_tempDir, name);
		File.WriteAllText(path, content);
		return path;
	}

	/// <summary>
	/// パス条件: AddFilesでファイルを取り込むと、DisplayedFilesに追加されリポジトリへ保存されること。
	/// </summary>
	[Fact]
	public void AddFiles_ファイルを取り込むとDisplayedFilesに追加され保存される()
	{
		var repository = new FakeTaggedFileRepository();
		var viewModel = new MainViewModel(repository);
		var path = CreateSampleFile("a.txt");

		viewModel.AddFiles([path]);

		Assert.Single(viewModel.Files);
		Assert.Single(viewModel.DisplayedFiles);
		Assert.Equal(path, viewModel.Files[0].FilePath);
		Assert.Equal(1, repository.SaveCallCount);
	}

	/// <summary>
	/// パス条件: 既に取り込み済みのパスを再度追加しても、重複して追加されないこと。
	/// </summary>
	[Fact]
	public void AddFiles_取り込み済みのパスは重複追加されない()
	{
		var viewModel = new MainViewModel(new FakeTaggedFileRepository());
		var path = CreateSampleFile("a.txt");

		viewModel.AddFiles([path]);
		viewModel.AddFiles([path]);

		Assert.Single(viewModel.Files);
	}

	/// <summary>
	/// パス条件: AddTagCommandを実行すると、選択中のファイルにタグが追加されること。
	/// </summary>
	[Fact]
	public void AddTagCommand_実行すると選択中のファイルにタグが追加される()
	{
		var viewModel = new MainViewModel(new FakeTaggedFileRepository());
		viewModel.AddFiles([CreateSampleFile("a.txt")]);
		viewModel.SelectedFile = viewModel.Files[0];
		viewModel.NewTagInput = "重要, 仕事";

		viewModel.AddTagCommand.Execute(null);

		Assert.Equal(["重要", "仕事"], viewModel.SelectedFile!.Tags);
		Assert.Equal(string.Empty, viewModel.NewTagInput);
	}

	/// <summary>
	/// パス条件: FilterTagを設定すると、そのタグを持つファイルのみDisplayedFilesに残ること。
	/// </summary>
	[Fact]
	public void FilterTag_設定するとそのタグを持つファイルのみ表示される()
	{
		var viewModel = new MainViewModel(new FakeTaggedFileRepository());
		viewModel.AddFiles([CreateSampleFile("a.txt"), CreateSampleFile("b.txt")]);
		viewModel.SelectedFile = viewModel.Files[0];
		viewModel.NewTagInput = "重要";
		viewModel.AddTagCommand.Execute(null);

		viewModel.FilterTag = "重要";

		Assert.Single(viewModel.DisplayedFiles);
		Assert.Equal("a.txt", viewModel.DisplayedFiles[0].FileName);
	}

	/// <summary>
	/// パス条件: RemoveFileCommandを実行すると、選択中のファイルが一覧から削除されること。
	/// </summary>
	[Fact]
	public void RemoveFileCommand_実行すると選択中のファイルが削除される()
	{
		var viewModel = new MainViewModel(new FakeTaggedFileRepository());
		viewModel.AddFiles([CreateSampleFile("a.txt")]);
		viewModel.SelectedFile = viewModel.Files[0];

		viewModel.RemoveFileCommand.Execute(null);

		Assert.Empty(viewModel.Files);
		Assert.Null(viewModel.SelectedFile);
	}

	/// <summary>
	/// パス条件: MoveFileで並び替えると、Files内の順序が入れ替わり保存されること。
	/// </summary>
	[Fact]
	public void MoveFile_並び替えるとFilesの順序が入れ替わる()
	{
		var repository = new FakeTaggedFileRepository();
		var viewModel = new MainViewModel(repository);
		viewModel.AddFiles([CreateSampleFile("a.txt"), CreateSampleFile("b.txt"), CreateSampleFile("c.txt")]);
		var first = viewModel.Files[0];
		var last = viewModel.Files[2];

		viewModel.MoveFile(first, last);

		Assert.Equal("b.txt", viewModel.Files[0].FileName);
		Assert.Equal("a.txt", viewModel.Files[2].FileName);
	}
}
