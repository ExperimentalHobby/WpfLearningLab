using System.Net;
using System.Text.Json;
using SelfHostedNoteSync.Client.Models;
using SelfHostedNoteSync.Client.Services;

namespace SelfHostedNoteSync.Client.Tests;

/// <summary>
/// <see cref="NoteApiClient"/> の単体テスト。
/// 実ネットワーク通信はせず、<see cref="FakeHttpMessageHandler"/>でJSON応答を差し替える。
/// </summary>
public class NoteApiClientTests
{
	/// <summary>
	/// パス条件: 正常なレスポンスからメモ一覧を取得できること。
	/// </summary>
	[Fact]
	public async Task GetNotesAsync_正常レスポンスからメモ一覧を取得できる()
	{
		const string json = """[{"id":1,"title":"買い物","content":"牛乳"}]""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(json));
		var client = new NoteApiClient(httpClient);

		var notes = await client.GetNotesAsync();

		Assert.Single(notes);
		Assert.Equal("買い物", notes[0].Title);
	}

	/// <summary>
	/// パス条件: メモ作成時にPOSTリクエストがJSONボディ付きで送信され、作成結果が返ること。
	/// </summary>
	[Fact]
	public async Task CreateNoteAsync_POSTでリクエストを送信し作成結果を返す()
	{
		const string json = """{"id":1,"title":"買い物","content":"牛乳"}""";
		HttpRequestMessage? capturedRequest = null;
		string? capturedBody = null;
		var handler = new FakeHttpMessageHandler(json, HttpStatusCode.Created, (req, body) =>
		{
			capturedRequest = req;
			capturedBody = body;
		});
		var client = new NoteApiClient(new HttpClient(handler));

		var created = await client.CreateNoteAsync("買い物", "牛乳");

		Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
		var sentInput = JsonSerializer.Deserialize<NoteInput>(capturedBody!, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		Assert.Equal("買い物", sentInput!.Title);
		Assert.Equal(1, created.Id);
	}

	/// <summary>
	/// パス条件: メモ更新時にPUTリクエストが対象IDのURLに送信されること。
	/// </summary>
	[Fact]
	public async Task UpdateNoteAsync_PUTでIDを含むURLにリクエストを送信する()
	{
		const string json = """{"id":5,"title":"買い物(更新)","content":"卵"}""";
		HttpRequestMessage? capturedRequest = null;
		var handler = new FakeHttpMessageHandler(json, HttpStatusCode.OK, (req, _) => capturedRequest = req);
		var client = new NoteApiClient(new HttpClient(handler));

		var updated = await client.UpdateNoteAsync(5, "買い物(更新)", "卵");

		Assert.Equal(HttpMethod.Put, capturedRequest!.Method);
		Assert.EndsWith("/notes/5", capturedRequest.RequestUri!.AbsolutePath);
		Assert.Equal("卵", updated.Content);
	}

	/// <summary>
	/// パス条件: メモ削除時にDELETEリクエストが対象IDのURLに送信されること。
	/// </summary>
	[Fact]
	public async Task DeleteNoteAsync_DELETEでIDを含むURLにリクエストを送信する()
	{
		HttpRequestMessage? capturedRequest = null;
		var handler = new FakeHttpMessageHandler(string.Empty, HttpStatusCode.NoContent, (req, _) => capturedRequest = req);
		var client = new NoteApiClient(new HttpClient(handler));

		await client.DeleteNoteAsync(5);

		Assert.Equal(HttpMethod.Delete, capturedRequest!.Method);
		Assert.EndsWith("/notes/5", capturedRequest.RequestUri!.AbsolutePath);
	}

	/// <summary>
	/// パス条件: サーバーがエラーステータスを返した場合、HttpRequestExceptionが送出されること。
	/// </summary>
	[Fact]
	public async Task GetNotesAsync_サーバーエラー時にHttpRequestExceptionを送出する()
	{
		var handler = new FakeHttpMessageHandler("", HttpStatusCode.InternalServerError);
		var client = new NoteApiClient(new HttpClient(handler));

		await Assert.ThrowsAsync<HttpRequestException>(() => client.GetNotesAsync());
	}
}
