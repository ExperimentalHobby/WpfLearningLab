using SelfHostedNoteSync.Client.Models;
using SelfHostedNoteSync.Client.ViewModels;

namespace SelfHostedNoteSync.Client.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: LoadCommand実行でAPIから取得したメモがNotesに反映されること。
	/// </summary>
	[Fact]
	public async Task LoadCommand_成功時にNotesへメモ一覧が反映される()
	{
		var fake = new FakeNoteApiClient
		{
			NotesResult = [new Note { Id = 1, Title = "買い物", Content = "牛乳" }],
		};
		var viewModel = new MainViewModel(fake);

		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);

		Assert.Single(viewModel.Notes);
		Assert.Equal("買い物", viewModel.Notes[0].Title);
		Assert.Equal(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: LoadCommand失敗時にErrorMessageが設定されること。
	/// </summary>
	[Fact]
	public async Task LoadCommand_失敗時にErrorMessageが設定される()
	{
		var fake = new FakeNoteApiClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = new MainViewModel(fake);

		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
		Assert.Empty(viewModel.Notes);
	}

	/// <summary>
	/// パス条件: AddCommand実行で作成されたメモがNotesに追加され、入力欄がクリアされること。
	/// </summary>
	[Fact]
	public async Task AddCommand_成功時にNotesへ追加され入力欄がクリアされる()
	{
		var fake = new FakeNoteApiClient
		{
			CreateResult = new Note { Id = 1, Title = "買い物", Content = "牛乳" },
		};
		var viewModel = new MainViewModel(fake)
		{
			TitleInput = "買い物",
			ContentInput = "牛乳",
		};

		viewModel.AddCommand.Execute(null);
		await Task.Delay(50);

		Assert.Single(viewModel.Notes);
		Assert.Equal(string.Empty, viewModel.TitleInput);
		Assert.Equal(string.Empty, viewModel.ContentInput);
	}

	/// <summary>
	/// パス条件: AddCommand失敗時にErrorMessageが設定され、Notesに追加されないこと。
	/// </summary>
	[Fact]
	public async Task AddCommand_失敗時にErrorMessageが設定されNotesに追加されない()
	{
		var fake = new FakeNoteApiClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = new MainViewModel(fake) { TitleInput = "買い物" };

		viewModel.AddCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
		Assert.Empty(viewModel.Notes);
	}

	/// <summary>
	/// パス条件: SelectedNoteを設定するとTitleInput/ContentInputに反映されること。
	/// </summary>
	[Fact]
	public void SelectedNote_設定するとTitleInputとContentInputに反映される()
	{
		var viewModel = new MainViewModel(new FakeNoteApiClient());
		var note = new Note { Id = 1, Title = "買い物", Content = "牛乳" };

		viewModel.SelectedNote = note;

		Assert.Equal("買い物", viewModel.TitleInput);
		Assert.Equal("牛乳", viewModel.ContentInput);
	}

	/// <summary>
	/// パス条件: UpdateCommand実行でNotes内の該当メモが更新後の内容に置き換わること。
	/// </summary>
	[Fact]
	public async Task UpdateCommand_成功時にNotes内の該当メモが置き換わる()
	{
		var original = new Note { Id = 1, Title = "買い物", Content = "牛乳" };
		var fake = new FakeNoteApiClient
		{
			NotesResult = [original],
			UpdateResult = new Note { Id = 1, Title = "買い物(更新)", Content = "卵" },
		};
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedNote = viewModel.Notes[0];
		viewModel.TitleInput = "買い物(更新)";
		viewModel.ContentInput = "卵";

		viewModel.UpdateCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal("買い物(更新)", viewModel.Notes[0].Title);
		Assert.Equal(1, fake.LastUpdateCall!.Value.Id);
	}

	/// <summary>
	/// パス条件: UpdateCommand実行後もSelectedNoteが更新後のメモを指し続けること。
	/// (Notes内の要素を新しい参照に置き換えると、ListBoxのSelectedItemバインディングが
	/// 古い参照を見失って選択解除してしまうため、明示的に選択を維持する必要がある)
	/// </summary>
	[Fact]
	public async Task UpdateCommand_成功後もSelectedNoteが更新後のメモを指す()
	{
		var original = new Note { Id = 1, Title = "買い物", Content = "牛乳" };
		var updated = new Note { Id = 1, Title = "買い物(更新)", Content = "卵" };
		var fake = new FakeNoteApiClient
		{
			NotesResult = [original],
			UpdateResult = updated,
		};
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedNote = viewModel.Notes[0];

		viewModel.UpdateCommand.Execute(null);
		await Task.Delay(50);

		Assert.Same(updated, viewModel.SelectedNote);
	}

	/// <summary>
	/// パス条件: UpdateCommand完了(IsBusyがfalseに戻った)後に、DeleteCommandのCanExecuteChangedが
	/// 発火し、その時点でCanExecuteがtrueになっていること。
	/// (IsBusyの変更を他コマンドに通知しないと、最後に発火したCanExecuteChangedの時点では
	/// まだIsBusy=trueのままで、WPFのButton.IsEnabledがfalseに固まってしまう)
	/// </summary>
	[Fact]
	public async Task UpdateCommand_完了後の最後のCanExecuteChanged時点でDeleteCommandが実行可能になっている()
	{
		var note = new Note { Id = 1, Title = "買い物", Content = "牛乳" };
		var fake = new FakeNoteApiClient
		{
			NotesResult = [note],
			UpdateResult = new Note { Id = 1, Title = "買い物(更新)", Content = "卵" },
		};
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedNote = viewModel.Notes[0];

		var canExecuteAtLastRaise = false;
		viewModel.DeleteCommand.CanExecuteChanged += (_, _) => canExecuteAtLastRaise = viewModel.DeleteCommand.CanExecute(null);

		viewModel.UpdateCommand.Execute(null);
		await Task.Delay(50);

		Assert.True(canExecuteAtLastRaise);
	}

	/// <summary>
	/// パス条件: DeleteCommand実行でNotesから該当メモが削除されSelectedNoteがnullになること。
	/// </summary>
	[Fact]
	public async Task DeleteCommand_成功時にNotesから削除されSelectedNoteがnullになる()
	{
		var note = new Note { Id = 1, Title = "買い物", Content = "牛乳" };
		var fake = new FakeNoteApiClient { NotesResult = [note] };
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedNote = viewModel.Notes[0];

		viewModel.DeleteCommand.Execute(null);
		await Task.Delay(50);

		Assert.Empty(viewModel.Notes);
		Assert.Null(viewModel.SelectedNote);
		Assert.Equal(1, fake.LastDeletedId);
	}
}
