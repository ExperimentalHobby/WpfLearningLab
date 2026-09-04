using AccessibleNoteApp.Models;
using AccessibleNoteApp.ViewModels;

namespace AccessibleNoteApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト。実ファイルI/Oは行わない<see cref="FakeMemoRepository"/>を使う。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: SaveCommandを実行すると新規メモがMemosに追加されリポジトリに保存されること
	/// </summary>
	[Fact]
	public void SaveCommand_未選択状態で実行すると新規メモが追加され保存される()
	{
		var repository = new FakeMemoRepository();
		var viewModel = new MainViewModel(repository);
		viewModel.TitleInput = "新しいメモ";
		viewModel.BodyInput = "本文";

		viewModel.SaveCommand.Execute(null);

		var memo = Assert.Single(viewModel.Memos);
		Assert.Equal("新しいメモ", memo.Title);
		Assert.Equal("本文", memo.Body);
		Assert.Single(repository.LoadAll());
	}

	/// <summary>
	/// パス条件: 既存メモを選択した状態でSaveCommandを実行すると、そのメモが更新されること
	/// </summary>
	[Fact]
	public void SaveCommand_既存メモ選択中に実行すると更新される()
	{
		var repository = new FakeMemoRepository();
		var viewModel = new MainViewModel(repository);
		viewModel.TitleInput = "元のタイトル";
		viewModel.SaveCommand.Execute(null);
		var created = viewModel.Memos[0];
		viewModel.SelectedMemo = created;

		viewModel.TitleInput = "更新後のタイトル";
		viewModel.SaveCommand.Execute(null);

		var memo = Assert.Single(viewModel.Memos);
		Assert.Equal(created.Id, memo.Id);
		Assert.Equal("更新後のタイトル", memo.Title);
	}

	/// <summary>
	/// パス条件: SelectedMemoを変更すると、TitleInput/BodyInputにその内容が反映されること
	/// </summary>
	[Fact]
	public void SelectedMemo_変更するとTitleInputとBodyInputに反映される()
	{
		var viewModel = new MainViewModel(new FakeMemoRepository());
		var memo = new Memo("id-1", "タイトル", "本文", DateTime.Now);

		viewModel.SelectedMemo = memo;

		Assert.Equal("タイトル", viewModel.TitleInput);
		Assert.Equal("本文", viewModel.BodyInput);
	}

	/// <summary>
	/// パス条件: NewMemoCommandを実行すると、選択が解除され入力欄がクリアされること
	/// </summary>
	[Fact]
	public void NewMemoCommand_実行すると選択解除され入力欄がクリアされる()
	{
		var viewModel = new MainViewModel(new FakeMemoRepository())
		{
			SelectedMemo = new Memo("id-1", "タイトル", "本文", DateTime.Now),
		};

		viewModel.NewMemoCommand.Execute(null);

		Assert.Null(viewModel.SelectedMemo);
		Assert.Equal(string.Empty, viewModel.TitleInput);
		Assert.Equal(string.Empty, viewModel.BodyInput);
	}

	/// <summary>
	/// パス条件: DeleteCommandを実行すると、選択中のメモがMemosから削除されリポジトリからも削除されること
	/// </summary>
	[Fact]
	public void DeleteCommand_実行すると選択中のメモが削除される()
	{
		var repository = new FakeMemoRepository();
		var viewModel = new MainViewModel(repository);
		viewModel.TitleInput = "削除対象";
		viewModel.SaveCommand.Execute(null);
		var created = viewModel.Memos[0];
		viewModel.SelectedMemo = created;

		viewModel.DeleteCommand.Execute(null);

		Assert.Empty(viewModel.Memos);
		Assert.Empty(repository.LoadAll());
		Assert.Null(viewModel.SelectedMemo);
	}

	/// <summary>
	/// パス条件: TitleInputが空の場合、SaveCommandのCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void SaveCommand_TitleInputが空の場合CanExecuteがfalseになる()
	{
		var viewModel = new MainViewModel(new FakeMemoRepository())
		{
			TitleInput = string.Empty,
		};

		Assert.False(viewModel.SaveCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 未選択の場合、DeleteCommandのCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void DeleteCommand_未選択の場合CanExecuteがfalseになる()
	{
		var viewModel = new MainViewModel(new FakeMemoRepository());

		Assert.False(viewModel.DeleteCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: SelectedIndexを設定すると、対応するメモがSelectedMemoに反映されること
	/// </summary>
	[Fact]
	public void SelectedIndex_設定すると対応するメモがSelectedMemoに反映される()
	{
		var repository = new FakeMemoRepository();
		var viewModel = new MainViewModel(repository);
		viewModel.TitleInput = "メモA";
		viewModel.SaveCommand.Execute(null);

		viewModel.SelectedIndex = 0;

		Assert.Equal(viewModel.Memos[0], viewModel.SelectedMemo);
	}

	/// <summary>
	/// パス条件: Load()を実行すると、リポジトリの内容がMemosに反映されること
	/// </summary>
	[Fact]
	public void Load_実行するとリポジトリの内容がMemosに反映される()
	{
		var repository = new FakeMemoRepository();
		repository.Save(new Memo("id-1", "既存メモ", "本文", DateTime.Now));
		var viewModel = new MainViewModel(repository);

		viewModel.Load();

		var memo = Assert.Single(viewModel.Memos);
		Assert.Equal("既存メモ", memo.Title);
	}
}
