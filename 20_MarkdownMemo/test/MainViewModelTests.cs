using MarkdownMemo.Services;
using MarkdownMemo.ViewModels;

namespace MarkdownMemo.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// フェイクの<see cref="FakeMemoRepository"/>と、実際の<see cref="MarkdigMarkdownToHtmlConverter"/>で検証する。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(FakeMemoRepository? repository = null) =>
		new(repository ?? new FakeMemoRepository(), new MarkdigMarkdownToHtmlConverter());

	/// <summary>
	/// パス条件: SaveCommand実行で入力内容が保存され、一覧に反映されること
	/// </summary>
	[Fact]
	public void SaveCommand_入力内容が保存され一覧に反映される()
	{
		var viewModel = CreateViewModel();
		viewModel.InputTitle = "買い物メモ";
		viewModel.MarkdownContent = "- 牛乳";

		viewModel.SaveCommand.Execute(null);

		Assert.Single(viewModel.Memos);
		Assert.Equal("買い物メモ", viewModel.Memos[0].Title);
	}

	/// <summary>
	/// パス条件: タイトルが空欄の場合、SaveCommandのCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void SaveCommand_タイトルが空欄だとCanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.InputTitle = "   ";

		Assert.False(viewModel.SaveCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: タイトルにファイル名として不正な文字(パス区切り文字等)が含まれる場合、
	/// SaveCommandのCanExecuteがfalseになること(パストラバーサル対策のUI側の防御)
	/// </summary>
	[Theory]
	[InlineData("../evil")]
	[InlineData("a/b")]
	[InlineData("a\\b")]
	[InlineData("a:b")]
	public void SaveCommand_不正な文字を含むタイトルはCanExecuteがfalseになる(string title)
	{
		var viewModel = CreateViewModel();
		viewModel.InputTitle = title;

		Assert.False(viewModel.SaveCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 一覧からメモを選択した際にLoadが例外(外部からファイルが削除された等)を
	/// 送出しても、クラッシュせずErrorMessageが設定されること
	/// </summary>
	[Fact]
	public void SelectedMemo_Load失敗時にクラッシュせずErrorMessageが設定される()
	{
		var repository = new FakeMemoRepository();
		repository.Save("買い物メモ", "- 牛乳");
		var viewModel = CreateViewModel(repository);
		repository.LoadExceptionToThrow = new IOException("ファイルが見つかりません。");

		var exception = Record.Exception(() => viewModel.SelectedMemo = viewModel.Memos[0]);

		Assert.Null(exception);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: コンストラクタ実行時にリポジトリの一覧が読み込まれること
	/// </summary>
	[Fact]
	public void コンストラクタ_起動時にリポジトリの一覧が読み込まれる()
	{
		var repository = new FakeMemoRepository();
		repository.Save("買い物メモ", "- 牛乳");

		var viewModel = CreateViewModel(repository);

		Assert.Single(viewModel.Memos);
		Assert.Equal("買い物メモ", viewModel.Memos[0].Title);
	}

	/// <summary>
	/// パス条件: MarkdownContentを変更するとPreviewHtmlが更新されること
	/// </summary>
	[Fact]
	public void MarkdownContent_変更するとPreviewHtmlが更新される()
	{
		var viewModel = CreateViewModel();

		viewModel.MarkdownContent = "# タイトル";

		Assert.Contains(">タイトル</h1>", viewModel.PreviewHtml);
	}

	/// <summary>
	/// パス条件: 一覧からメモを選択すると、InputTitle/MarkdownContentに内容が読み込まれること
	/// </summary>
	[Fact]
	public void SelectedMemo_選択すると編集欄に内容が読み込まれる()
	{
		var repository = new FakeMemoRepository();
		repository.Save("買い物メモ", "- 牛乳");
		var viewModel = CreateViewModel(repository);

		viewModel.SelectedMemo = viewModel.Memos[0];

		Assert.Equal("買い物メモ", viewModel.InputTitle);
		Assert.Equal("- 牛乳", viewModel.MarkdownContent);
	}

	/// <summary>
	/// パス条件: NewCommand実行で入力欄・選択状態がクリアされること
	/// </summary>
	[Fact]
	public void NewCommand_実行すると入力欄と選択状態がクリアされる()
	{
		var repository = new FakeMemoRepository();
		repository.Save("買い物メモ", "- 牛乳");
		var viewModel = CreateViewModel(repository);
		viewModel.SelectedMemo = viewModel.Memos[0];

		viewModel.NewCommand.Execute(null);

		Assert.Null(viewModel.SelectedMemo);
		Assert.Equal(string.Empty, viewModel.InputTitle);
		Assert.Equal(string.Empty, viewModel.MarkdownContent);
	}

	/// <summary>
	/// パス条件: DeleteCommand実行で選択中のメモが一覧・リポジトリから削除されること
	/// </summary>
	[Fact]
	public void DeleteCommand_選択中のメモが削除される()
	{
		var repository = new FakeMemoRepository();
		repository.Save("買い物メモ", "- 牛乳");
		var viewModel = CreateViewModel(repository);
		viewModel.SelectedMemo = viewModel.Memos[0];

		viewModel.DeleteCommand.Execute(null);

		Assert.Empty(viewModel.Memos);
		Assert.Empty(repository.GetAll());
	}

	/// <summary>
	/// パス条件: メモが未選択の場合、DeleteCommandのCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void DeleteCommand_未選択の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();

		Assert.False(viewModel.DeleteCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 保存後、一覧の選択状態が保存したメモに追従すること
	/// </summary>
	[Fact]
	public void SaveCommand_保存後に一覧の選択状態が追従する()
	{
		var viewModel = CreateViewModel();
		viewModel.InputTitle = "買い物メモ";
		viewModel.MarkdownContent = "- 牛乳";

		viewModel.SaveCommand.Execute(null);

		Assert.NotNull(viewModel.SelectedMemo);
		Assert.Equal("買い物メモ", viewModel.SelectedMemo!.Title);
	}
}
