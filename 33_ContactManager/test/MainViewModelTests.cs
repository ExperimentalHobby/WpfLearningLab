using ContactManager.Models;
using ContactManager.ViewModels;

namespace ContactManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: コンストラクタでリポジトリから連絡先一覧が読み込まれること。
	/// </summary>
	[Fact]
	public void Constructor_リポジトリから連絡先一覧を読み込む()
	{
		var repository = new FakeContactRepository();
		repository.Add(new Contact { Name = "既存の連絡先" });

		var viewModel = new MainViewModel(repository);

		Assert.Single(viewModel.Contacts);
		Assert.Equal("既存の連絡先", viewModel.Contacts[0].Name);
	}

	/// <summary>
	/// パス条件: 氏名が未入力の場合、AddCommandが実行不可であること。
	/// </summary>
	[Fact]
	public void AddCommand_氏名が未入力の場合実行不可()
	{
		var viewModel = new MainViewModel(new FakeContactRepository());

		Assert.False(viewModel.AddCommand.CanExecute(null));

		viewModel.NewName = "新規連絡先";
		Assert.True(viewModel.AddCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: AddCommandを実行すると、連絡先が一覧に追加されフォームがクリアされること。
	/// </summary>
	[Fact]
	public void AddCommand_実行すると連絡先が追加されフォームがクリアされる()
	{
		var viewModel = new MainViewModel(new FakeContactRepository())
		{
			NewName = "新規連絡先",
			NewPhoneNumber = "090-0000-0000",
			NewEmail = "new@example.com",
		};

		viewModel.AddCommand.Execute(null);

		Assert.Single(viewModel.Contacts);
		Assert.Equal("新規連絡先", viewModel.Contacts[0].Name);
		Assert.Equal(string.Empty, viewModel.NewName);
	}

	/// <summary>
	/// パス条件: 連絡先が未選択の場合、Update/DeleteCommandが実行不可であること。
	/// </summary>
	[Fact]
	public void UpdateAndDeleteCommand_未選択の場合実行不可()
	{
		var viewModel = new MainViewModel(new FakeContactRepository());

		Assert.False(viewModel.UpdateCommand.CanExecute(null));
		Assert.False(viewModel.DeleteCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: DeleteCommandを実行すると、選択中の連絡先が一覧から削除され選択が解除されること。
	/// </summary>
	[Fact]
	public void DeleteCommand_実行すると一覧から削除され選択が解除される()
	{
		var repository = new FakeContactRepository();
		repository.Add(new Contact { Name = "削除対象" });
		var viewModel = new MainViewModel(repository) { SelectedContact = repository.GetAll()[0] };

		viewModel.DeleteCommand.Execute(null);

		Assert.Empty(viewModel.Contacts);
		Assert.Null(viewModel.SelectedContact);
	}
}
