using System.Text.Json;
using SelfHostedNoteSync.Server;
using SelfHostedNoteSync.Server.Models;

namespace SelfHostedNoteSync.Server.Tests;

public class NoteApiHandlerTests
{
	/// <summary>
	/// パス条件: メモが1件もない状態でGET /notesを呼ぶと200と空配列が返ること。
	/// </summary>
	[Fact]
	public void Handle_GET_notes_メモがなければ200と空配列が返る()
	{
		var handler = new NoteApiHandler(new NoteStore());

		var response = handler.Handle(new ApiRequest("GET", "/notes", null));

		Assert.Equal(200, response.StatusCode);
		var notes = JsonSerializer.Deserialize<List<Note>>(response.Body!, JsonOptions);
		Assert.Empty(notes!);
	}

	/// <summary>
	/// パス条件: メモが存在する状態でGET /notes/{id}を呼ぶと200とそのメモが返ること。
	/// </summary>
	[Fact]
	public void Handle_GET_notes_id_存在するIDなら200とメモが返る()
	{
		var store = new NoteStore();
		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });
		var handler = new NoteApiHandler(store);

		var response = handler.Handle(new ApiRequest("GET", $"/notes/{added.Id}", null));

		Assert.Equal(200, response.StatusCode);
		var note = JsonSerializer.Deserialize<Note>(response.Body!, JsonOptions);
		Assert.Equal("買い物", note!.Title);
	}

	/// <summary>
	/// パス条件: 存在しないIDでGET /notes/{id}を呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public void Handle_GET_notes_id_存在しないIDなら404が返る()
	{
		var handler = new NoteApiHandler(new NoteStore());

		var response = handler.Handle(new ApiRequest("GET", "/notes/999", null));

		Assert.Equal(404, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 正しいJSONボディでPOST /notesを呼ぶと201でメモが作成されること。
	/// </summary>
	[Fact]
	public void Handle_POST_notes_正しいボディなら201でメモが作成される()
	{
		var store = new NoteStore();
		var handler = new NoteApiHandler(store);
		var body = JsonSerializer.Serialize(new NoteInput { Title = "買い物", Content = "牛乳" });

		var response = handler.Handle(new ApiRequest("POST", "/notes", body));

		Assert.Equal(201, response.StatusCode);
		var created = JsonSerializer.Deserialize<Note>(response.Body!, JsonOptions);
		Assert.True(created!.Id > 0);
		Assert.Single(store.GetAll());
	}

	/// <summary>
	/// パス条件: タイトルが空のボディでPOST /notesを呼ぶと400が返り、メモが作成されないこと。
	/// </summary>
	[Fact]
	public void Handle_POST_notes_タイトルが空なら400が返る()
	{
		var store = new NoteStore();
		var handler = new NoteApiHandler(store);
		var body = JsonSerializer.Serialize(new NoteInput { Title = "", Content = "牛乳" });

		var response = handler.Handle(new ApiRequest("POST", "/notes", body));

		Assert.Equal(400, response.StatusCode);
		Assert.Empty(store.GetAll());
	}

	/// <summary>
	/// パス条件: 不正なJSONでPOST /notesを呼ぶと400が返ること。
	/// </summary>
	[Fact]
	public void Handle_POST_notes_不正なJSONなら400が返る()
	{
		var handler = new NoteApiHandler(new NoteStore());

		var response = handler.Handle(new ApiRequest("POST", "/notes", "{ 不正なJSON"));

		Assert.Equal(400, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在するIDに正しいボディでPUT /notes/{id}を呼ぶと200で更新されること。
	/// </summary>
	[Fact]
	public void Handle_PUT_notes_id_存在するIDなら200で更新される()
	{
		var store = new NoteStore();
		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });
		var handler = new NoteApiHandler(store);
		var body = JsonSerializer.Serialize(new NoteInput { Title = "買い物(更新)", Content = "卵" });

		var response = handler.Handle(new ApiRequest("PUT", $"/notes/{added.Id}", body));

		Assert.Equal(200, response.StatusCode);
		Assert.Equal("買い物(更新)", store.GetById(added.Id)!.Title);
	}

	/// <summary>
	/// パス条件: 存在しないIDでPUT /notes/{id}を呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public void Handle_PUT_notes_id_存在しないIDなら404が返る()
	{
		var handler = new NoteApiHandler(new NoteStore());
		var body = JsonSerializer.Serialize(new NoteInput { Title = "買い物", Content = "牛乳" });

		var response = handler.Handle(new ApiRequest("PUT", "/notes/999", body));

		Assert.Equal(404, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在するIDでDELETE /notes/{id}を呼ぶと204でメモが削除されること。
	/// </summary>
	[Fact]
	public void Handle_DELETE_notes_id_存在するIDなら204で削除される()
	{
		var store = new NoteStore();
		var added = store.Add(new Note { Title = "買い物", Content = "牛乳" });
		var handler = new NoteApiHandler(store);

		var response = handler.Handle(new ApiRequest("DELETE", $"/notes/{added.Id}", null));

		Assert.Equal(204, response.StatusCode);
		Assert.Null(store.GetById(added.Id));
	}

	/// <summary>
	/// パス条件: 存在しないIDでDELETE /notes/{id}を呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public void Handle_DELETE_notes_id_存在しないIDなら404が返る()
	{
		var handler = new NoteApiHandler(new NoteStore());

		var response = handler.Handle(new ApiRequest("DELETE", "/notes/999", null));

		Assert.Equal(404, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 未知のパスにアクセスすると404が返ること。
	/// </summary>
	[Fact]
	public void Handle_未知のパスなら404が返る()
	{
		var handler = new NoteApiHandler(new NoteStore());

		var response = handler.Handle(new ApiRequest("GET", "/unknown", null));

		Assert.Equal(404, response.StatusCode);
	}

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}
