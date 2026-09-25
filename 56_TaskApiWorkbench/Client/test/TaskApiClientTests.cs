using System.Net;
using System.Text.Json;
using TaskApiWorkbench.Client.Models;
using TaskApiWorkbench.Client.Services;

namespace TaskApiWorkbench.Client.Tests;

/// <summary>
/// <see cref="TaskApiClient"/> の単体テスト。
/// 実ネットワーク通信はせず、<see cref="FakeHttpMessageHandler"/>でJSON応答を差し替える。
/// </summary>
public class TaskApiClientTests
{
	/// <summary>
	/// パス条件: 正常なレスポンスからタスク一覧を取得できること。
	/// </summary>
	[Fact]
	public async Task GetTasksAsync_正常レスポンスからタスク一覧を取得できる()
	{
		const string json = """[{"id":1,"title":"資料整理","description":"","isCompleted":false}]""";
		var client = new TaskApiClient(new HttpClient(new FakeHttpMessageHandler(json)));

		var tasks = await client.GetTasksAsync();

		Assert.Single(tasks);
		Assert.Equal("資料整理", tasks[0].Title);
	}

	/// <summary>
	/// パス条件: タスク作成時にPOSTリクエストがJSONボディ付きで送信され、作成結果が返ること。
	/// </summary>
	[Fact]
	public async Task CreateTaskAsync_POSTでリクエストを送信し作成結果を返す()
	{
		const string json = """{"id":1,"title":"資料整理","description":"","isCompleted":false}""";
		HttpRequestMessage? capturedRequest = null;
		string? capturedBody = null;
		var handler = new FakeHttpMessageHandler(json, HttpStatusCode.Created, (req, body) =>
		{
			capturedRequest = req;
			capturedBody = body;
		});
		var client = new TaskApiClient(new HttpClient(handler));

		var created = await client.CreateTaskAsync("資料整理", "");

		Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
		var sentInput = JsonSerializer.Deserialize<TaskItemRequest>(capturedBody!, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		Assert.Equal("資料整理", sentInput!.Title);
		Assert.Equal(1, created.Id);
	}

	/// <summary>
	/// パス条件: タスク更新時にPUTリクエストが対象IDのURLに送信されること。
	/// </summary>
	[Fact]
	public async Task UpdateTaskAsync_PUTでIDを含むURLにリクエストを送信する()
	{
		const string json = """{"id":5,"title":"資料整理(更新)","description":"追記","isCompleted":false}""";
		HttpRequestMessage? capturedRequest = null;
		var handler = new FakeHttpMessageHandler(json, HttpStatusCode.OK, (req, _) => capturedRequest = req);
		var client = new TaskApiClient(new HttpClient(handler));

		var updated = await client.UpdateTaskAsync(5, "資料整理(更新)", "追記");

		Assert.Equal(HttpMethod.Put, capturedRequest!.Method);
		Assert.EndsWith("/api/tasks/5", capturedRequest.RequestUri!.AbsolutePath);
		Assert.Equal("追記", updated.Description);
	}

	/// <summary>
	/// パス条件: 完了状態トグル時にPATCHリクエストが対象IDのURLに送信されること。
	/// </summary>
	[Fact]
	public async Task ToggleCompleteAsync_PATCHでIDを含むURLにリクエストを送信する()
	{
		const string json = """{"id":5,"title":"資料整理","description":"","isCompleted":true}""";
		HttpRequestMessage? capturedRequest = null;
		var handler = new FakeHttpMessageHandler(json, HttpStatusCode.OK, (req, _) => capturedRequest = req);
		var client = new TaskApiClient(new HttpClient(handler));

		var toggled = await client.ToggleCompleteAsync(5);

		Assert.Equal(HttpMethod.Patch, capturedRequest!.Method);
		Assert.EndsWith("/api/tasks/5/complete", capturedRequest.RequestUri!.AbsolutePath);
		Assert.True(toggled.IsCompleted);
	}

	/// <summary>
	/// パス条件: タスク削除時にDELETEリクエストが対象IDのURLに送信されること。
	/// </summary>
	[Fact]
	public async Task DeleteTaskAsync_DELETEでIDを含むURLにリクエストを送信する()
	{
		HttpRequestMessage? capturedRequest = null;
		var handler = new FakeHttpMessageHandler(string.Empty, HttpStatusCode.NoContent, (req, _) => capturedRequest = req);
		var client = new TaskApiClient(new HttpClient(handler));

		await client.DeleteTaskAsync(5);

		Assert.Equal(HttpMethod.Delete, capturedRequest!.Method);
		Assert.EndsWith("/api/tasks/5", capturedRequest.RequestUri!.AbsolutePath);
	}

	/// <summary>
	/// パス条件: サーバーがエラーステータスを返した場合、HttpRequestExceptionが送出されること。
	/// </summary>
	[Fact]
	public async Task GetTasksAsync_サーバーエラー時にHttpRequestExceptionを送出する()
	{
		var handler = new FakeHttpMessageHandler("", HttpStatusCode.InternalServerError);
		var client = new TaskApiClient(new HttpClient(handler));

		await Assert.ThrowsAsync<HttpRequestException>(() => client.GetTasksAsync());
	}
}
