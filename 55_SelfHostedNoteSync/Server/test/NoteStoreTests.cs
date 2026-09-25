using SelfHostedNoteSync.Server;
using SelfHostedNoteSync.Server.Models;

namespace SelfHostedNoteSync.Server.Tests;

public class NoteStoreTests
{
	/// <summary>
	/// パス条件: Addで追加したメモにIDが採番され、GetAllに含まれること。
	/// </summary>
	[Fact]
	public void Add_採番したIDでメモが追加されGetAllに含まれる()
	{
		var store = new NoteStore();

		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });

		Assert.True(added.Id > 0);
		Assert.Contains(store.GetAll(), n => n.Id == added.Id && n.Title == "買い物");
	}

	/// <summary>
	/// パス条件: 存在するIDでGetByIdを呼ぶとそのメモが返ること。
	/// </summary>
	[Fact]
	public void GetById_存在するIDならメモが返る()
	{
		var store = new NoteStore();
		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });

		var found = store.GetById(added.Id);

		Assert.NotNull(found);
		Assert.Equal("買い物", found!.Title);
	}

	/// <summary>
	/// パス条件: 存在しないIDでGetByIdを呼ぶとnullが返ること。
	/// </summary>
	[Fact]
	public void GetById_存在しないIDならnullが返る()
	{
		var store = new NoteStore();

		var found = store.GetById(999);

		Assert.Null(found);
	}

	/// <summary>
	/// パス条件: 存在するIDでUpdateを呼ぶと内容が更新されtrueが返ること。
	/// </summary>
	[Fact]
	public void Update_存在するIDなら内容が更新されtrueが返る()
	{
		var store = new NoteStore();
		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });

		var updated = store.Update(added.Id, new Note { Title = "買い物(更新)", Content = "卵" });

		Assert.True(updated);
		var found = store.GetById(added.Id);
		Assert.Equal("買い物(更新)", found!.Title);
		Assert.Equal("卵", found.Content);
	}

	/// <summary>
	/// パス条件: 存在しないIDでUpdateを呼ぶとfalseが返り、内容も追加されないこと。
	/// </summary>
	[Fact]
	public void Update_存在しないIDならfalseが返る()
	{
		var store = new NoteStore();

		var updated = store.Update(999, new Note { Title = "買い物", Content = "牛乳" });

		Assert.False(updated);
		Assert.Empty(store.GetAll());
	}

	/// <summary>
	/// パス条件: 存在するIDでDeleteを呼ぶとメモが削除されtrueが返ること。
	/// </summary>
	[Fact]
	public void Delete_存在するIDならメモが削除されtrueが返る()
	{
		var store = new NoteStore();
		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });

		var deleted = store.Delete(added.Id);

		Assert.True(deleted);
		Assert.Null(store.GetById(added.Id));
	}

	/// <summary>
	/// パス条件: 存在しないIDでDeleteを呼ぶとfalseが返ること。
	/// </summary>
	[Fact]
	public void Delete_存在しないIDならfalseが返る()
	{
		var store = new NoteStore();

		var deleted = store.Delete(999);

		Assert.False(deleted);
	}
}
