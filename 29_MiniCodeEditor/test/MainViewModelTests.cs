using System.IO;
using MiniCodeEditor.ViewModels;

namespace MiniCodeEditor.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakeEditorController? editor = null,
		FakeFileDialogService? dialog = null,
		FakeFileService? fileService = null,
		FakeUnsavedChangesPrompt? unsavedChangesPrompt = null) =>
		new(
			editor ?? new FakeEditorController(),
			dialog ?? new FakeFileDialogService(),
			fileService ?? new FakeFileService(),
			unsavedChangesPrompt ?? new FakeUnsavedChangesPrompt());

	/// <summary>
	/// パス条件: OpenCommand実行で、ダイアログが返したパスの内容がエディタに反映されCurrentFilePathが設定されること
	/// </summary>
	[Fact]
	public void OpenCommand_実行するとダイアログが返したパスの内容がエディタに反映される()
	{
		var editor = new FakeEditorController();
		var dialog = new FakeFileDialogService { OpenDialogResult = @"C:\src\sample.cs" };
		var fileService = new FakeFileService();
		fileService.SeedFile(@"C:\src\sample.cs", "class C { }");
		var viewModel = CreateViewModel(editor, dialog, fileService);

		viewModel.OpenCommand.Execute(null);

		Assert.Equal("class C { }", editor.Text);
		Assert.Equal(@"C:\src\sample.cs", viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: OpenCommand実行で、拡張子に応じたシンタックスハイライトが設定されること
	/// </summary>
	[Fact]
	public void OpenCommand_実行すると拡張子に応じたシンタックスハイライトが設定される()
	{
		var editor = new FakeEditorController();
		var dialog = new FakeFileDialogService { OpenDialogResult = @"C:\src\sample.cs" };
		var fileService = new FakeFileService();
		fileService.SeedFile(@"C:\src\sample.cs", "class C { }");
		var viewModel = CreateViewModel(editor, dialog, fileService);

		viewModel.OpenCommand.Execute(null);

		Assert.Equal(@"C:\src\sample.cs", editor.LastSyntaxHighlightingFilePath);
	}

	/// <summary>
	/// パス条件: OpenCommand実行時にダイアログがキャンセルされた(nullを返す)場合、何もしないこと
	/// </summary>
	[Fact]
	public void OpenCommand_ダイアログがキャンセルされた場合何もしない()
	{
		var editor = new FakeEditorController { Text = "元の内容" };
		var dialog = new FakeFileDialogService { OpenDialogResult = null };
		var viewModel = CreateViewModel(editor, dialog);

		viewModel.OpenCommand.Execute(null);

		Assert.Equal("元の内容", editor.Text);
		Assert.Null(viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: CurrentFilePath未設定の状態でSaveCommandを実行すると、
	/// ダイアログが表示されそこで指定したパスに保存されCurrentFilePathが更新されること
	/// </summary>
	[Fact]
	public void SaveCommand_CurrentFilePath未設定の場合ダイアログで指定したパスに保存される()
	{
		var editor = new FakeEditorController { Text = "content" };
		var dialog = new FakeFileDialogService { SaveDialogResult = @"C:\src\new.cs" };
		var fileService = new FakeFileService();
		var viewModel = CreateViewModel(editor, dialog, fileService);

		viewModel.SaveCommand.Execute(null);

		Assert.Equal(@"C:\src\new.cs", fileService.LastWrittenPath);
		Assert.Equal("content", fileService.LastWrittenContent);
		Assert.Equal(@"C:\src\new.cs", viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: CurrentFilePath設定済みの状態でSaveCommandを実行すると、
	/// ダイアログを表示せずそのパスに保存されること
	/// </summary>
	[Fact]
	public void SaveCommand_CurrentFilePath設定済みの場合ダイアログを表示せず保存される()
	{
		var editor = new FakeEditorController();
		var dialog = new FakeFileDialogService { OpenDialogResult = @"C:\src\sample.cs" };
		var fileService = new FakeFileService();
		fileService.SeedFile(@"C:\src\sample.cs", "元の内容");
		var viewModel = CreateViewModel(editor, dialog, fileService);
		viewModel.OpenCommand.Execute(null);

		// ファイルを開いた後にエディタ上で編集した状態を模擬する
		editor.Text = "更新後の内容";
		dialog.SaveDialogResult = @"C:\should-not-be-used.cs";
		viewModel.SaveCommand.Execute(null);

		Assert.Equal(@"C:\src\sample.cs", fileService.LastWrittenPath);
		Assert.Equal("更新後の内容", fileService.LastWrittenContent);
	}

	/// <summary>
	/// パス条件: SaveAsCommand実行で、ダイアログが返したパスに保存されCurrentFilePathが更新されること
	/// </summary>
	[Fact]
	public void SaveAsCommand_実行するとダイアログが返したパスに保存される()
	{
		var editor = new FakeEditorController { Text = "content" };
		var dialog = new FakeFileDialogService { SaveDialogResult = @"C:\src\saved-as.cs" };
		var fileService = new FakeFileService();
		var viewModel = CreateViewModel(editor, dialog, fileService);

		viewModel.SaveAsCommand.Execute(null);

		Assert.Equal(@"C:\src\saved-as.cs", fileService.LastWrittenPath);
		Assert.Equal(@"C:\src\saved-as.cs", viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: SaveAsCommand実行時にダイアログがキャンセルされた場合、何も保存しないこと
	/// </summary>
	[Fact]
	public void SaveAsCommand_ダイアログがキャンセルされた場合何も保存しない()
	{
		var fileService = new FakeFileService();
		var dialog = new FakeFileDialogService { SaveDialogResult = null };
		var viewModel = CreateViewModel(dialog: dialog, fileService: fileService);

		viewModel.SaveAsCommand.Execute(null);

		Assert.Null(fileService.LastWrittenPath);
		Assert.Null(viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: NewCommand実行で、エディタとCurrentFilePathがクリアされること
	/// </summary>
	[Fact]
	public void NewCommand_実行するとエディタとCurrentFilePathがクリアされる()
	{
		var editor = new FakeEditorController();
		var dialog = new FakeFileDialogService { OpenDialogResult = @"C:\src\sample.cs" };
		var fileService = new FakeFileService();
		fileService.SeedFile(@"C:\src\sample.cs", "既存の内容");
		var viewModel = CreateViewModel(editor, dialog, fileService);
		viewModel.OpenCommand.Execute(null);

		viewModel.NewCommand.Execute(null);

		Assert.Equal(string.Empty, editor.Text);
		Assert.Null(viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: エディタのTextChangedが発火するとIsDirtyがtrueになること
	/// </summary>
	[Fact]
	public void TextChanged_発火するとIsDirtyがtrueになる()
	{
		var editor = new FakeEditorController();
		var viewModel = CreateViewModel(editor);

		editor.RaiseTextChanged();

		Assert.True(viewModel.IsDirty);
	}

	/// <summary>
	/// パス条件: 未保存の変更がある状態でNewCommandを実行し、確認ダイアログで「破棄」を選ぶと
	/// そのまま新規作成されること
	/// </summary>
	[Fact]
	public void NewCommand_未保存の変更があり破棄を選ぶと新規作成される()
	{
		var editor = new FakeEditorController { Text = "編集中の内容" };
		var prompt = new FakeUnsavedChangesPrompt { ResultToReturn = false };
		var viewModel = CreateViewModel(editor, unsavedChangesPrompt: prompt);
		editor.RaiseTextChanged();

		viewModel.NewCommand.Execute(null);

		Assert.Equal(1, prompt.CallCount);
		Assert.Equal(string.Empty, editor.Text);
		Assert.False(viewModel.IsDirty);
	}

	/// <summary>
	/// パス条件: 未保存の変更がある状態でNewCommandを実行し、確認ダイアログで「キャンセル」を選ぶと
	/// 何も変更されないこと
	/// </summary>
	[Fact]
	public void NewCommand_未保存の変更がありキャンセルを選ぶと何も変更されない()
	{
		var editor = new FakeEditorController { Text = "編集中の内容" };
		var prompt = new FakeUnsavedChangesPrompt { ResultToReturn = null };
		var viewModel = CreateViewModel(editor, unsavedChangesPrompt: prompt);
		editor.RaiseTextChanged();

		viewModel.NewCommand.Execute(null);

		Assert.Equal("編集中の内容", editor.Text);
		Assert.True(viewModel.IsDirty);
	}

	/// <summary>
	/// パス条件: 未保存の変更がある状態でOpenCommandを実行し、確認ダイアログで「保存」を選ぶと
	/// 保存してから開かれること
	/// </summary>
	[Fact]
	public void OpenCommand_未保存の変更があり保存を選ぶと保存してから開かれる()
	{
		var editor = new FakeEditorController { Text = "編集中の内容" };
		var dialog = new FakeFileDialogService
		{
			SaveDialogResult = @"C:\src\old.cs",
			OpenDialogResult = @"C:\src\new.cs",
		};
		var fileService = new FakeFileService();
		fileService.SeedFile(@"C:\src\new.cs", "新しい内容");
		var prompt = new FakeUnsavedChangesPrompt { ResultToReturn = true };
		var viewModel = CreateViewModel(editor, dialog, fileService, prompt);
		editor.RaiseTextChanged();

		viewModel.OpenCommand.Execute(null);

		Assert.Equal(@"C:\src\old.cs", fileService.LastWrittenPath);
		Assert.Equal("編集中の内容", fileService.LastWrittenContent);
		Assert.Equal("新しい内容", editor.Text);
		Assert.Equal(@"C:\src\new.cs", viewModel.CurrentFilePath);
	}

	/// <summary>
	/// パス条件: OpenCommand実行時にファイルの読込に失敗しても、クラッシュせずErrorMessageが設定されること
	/// </summary>
	[Fact]
	public void OpenCommand_読込に失敗してもクラッシュせずErrorMessageが設定される()
	{
		var dialog = new FakeFileDialogService { OpenDialogResult = @"C:\src\not-exist.cs" };
		var viewModel = CreateViewModel(dialog: dialog);

		viewModel.OpenCommand.Execute(null);

		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}

	/// <summary>
	/// パス条件: SaveCommand実行時に保存に失敗しても、クラッシュせずErrorMessageが設定されること
	/// </summary>
	[Fact]
	public void SaveCommand_保存に失敗してもクラッシュせずErrorMessageが設定される()
	{
		var dialog = new FakeFileDialogService { SaveDialogResult = @"C:\src\new.cs" };
		var fileService = new FakeFileService { WriteExceptionToThrow = new IOException("ディスク容量不足です") };
		var viewModel = CreateViewModel(dialog: dialog, fileService: fileService);

		viewModel.SaveCommand.Execute(null);

		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}
}
