using System.Net;
using System.Net.Http.Json;
using TaskApiWorkbench.Server.Dtos;

namespace TaskApiWorkbench.Server.Tests;

/// <summary>
/// タスクCRUD APIエンドポイントの結合テスト。
/// <see cref="TaskApiFactory"/>でテスト用サーバーを起動し、実際のHTTPリクエストで検証する。
/// テストごとに独立したInMemoryデータベースを使うため、テストメソッドごとに新しい
/// <see cref="TaskApiFactory"/>を生成する(<see cref="IClassFixture{TFixture}"/>で共有すると、
/// 他のテストが作成したデータが残ったままになり「メモがなければ空」のようなテストが失敗する)。
/// </summary>
public class TaskEndpointsTests : IDisposable
{
	private readonly TaskApiFactory _factory = new();
	private readonly HttpClient _client;

	/// <summary>テストごとに独立したサーバーとHTTPクライアントを準備する。</summary>
	public TaskEndpointsTests()
	{
		_client = _factory.CreateClient();
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_client.Dispose();
		_factory.Dispose();
	}

	/// <summary>
	/// パス条件: タスクが1件もない状態でGET /api/tasksを呼ぶと200と空配列が返ること。
	/// </summary>
	[Fact]
	public async Task GetAll_タスクがなければ200と空配列が返る()
	{
		var response = await _client.GetAsync("/api/tasks");

		response.EnsureSuccessStatusCode();
		var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>();
		Assert.Empty(tasks!);
	}

	/// <summary>
	/// パス条件: 正しいTitleでPOST /api/tasksを呼ぶと201でタスクが作成されること。
	/// </summary>
	[Fact]
	public async Task Create_正しいTitleなら201でタスクが作成される()
	{
		var response = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "報告書作成", Description = "月次報告" });

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		var created = await response.Content.ReadFromJsonAsync<TaskItemDto>();
		Assert.True(created!.Id > 0);
		Assert.Equal("報告書作成", created.Title);
		Assert.False(created.IsCompleted);
	}

	/// <summary>
	/// パス条件: Titleが空でPOST /api/tasksを呼ぶと400が返ること。
	/// </summary>
	[Fact]
	public async Task Create_Titleが空なら400が返る()
	{
		var response = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "", Description = "説明のみ" });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在するIDでGET /api/tasks/{id}を呼ぶと200とそのタスクが返ること。
	/// </summary>
	[Fact]
	public async Task GetById_存在するIDなら200とタスクが返る()
	{
		var createResponse = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "資料整理" });
		var created = await createResponse.Content.ReadFromJsonAsync<TaskItemDto>();

		var response = await _client.GetAsync($"/api/tasks/{created!.Id}");

		response.EnsureSuccessStatusCode();
		var task = await response.Content.ReadFromJsonAsync<TaskItemDto>();
		Assert.Equal("資料整理", task!.Title);
	}

	/// <summary>
	/// パス条件: 存在しないIDでGET /api/tasks/{id}を呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public async Task GetById_存在しないIDなら404が返る()
	{
		var response = await _client.GetAsync("/api/tasks/999999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在するIDに正しいTitleでPUT /api/tasks/{id}を呼ぶと200で更新されること。
	/// </summary>
	[Fact]
	public async Task Update_存在するIDなら200で更新される()
	{
		var createResponse = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "資料整理" });
		var created = await createResponse.Content.ReadFromJsonAsync<TaskItemDto>();

		var response = await _client.PutAsJsonAsync($"/api/tasks/{created!.Id}", new TaskItemRequest { Title = "資料整理(更新)", Description = "追記" });

		response.EnsureSuccessStatusCode();
		var updated = await response.Content.ReadFromJsonAsync<TaskItemDto>();
		Assert.Equal("資料整理(更新)", updated!.Title);
		Assert.Equal("追記", updated.Description);
	}

	/// <summary>
	/// パス条件: 存在しないIDでPUT /api/tasks/{id}を呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public async Task Update_存在しないIDなら404が返る()
	{
		var response = await _client.PutAsJsonAsync("/api/tasks/999999", new TaskItemRequest { Title = "資料整理" });

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	/// <summary>
	/// パス条件: Titleが空でPUT /api/tasks/{id}を呼ぶと400が返ること。
	/// </summary>
	[Fact]
	public async Task Update_Titleが空なら400が返る()
	{
		var createResponse = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "資料整理" });
		var created = await createResponse.Content.ReadFromJsonAsync<TaskItemDto>();

		var response = await _client.PutAsJsonAsync($"/api/tasks/{created!.Id}", new TaskItemRequest { Title = "" });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在するIDでPATCH /api/tasks/{id}/completeを呼ぶと完了状態がトグルされ200が返ること。
	/// </summary>
	[Fact]
	public async Task ToggleComplete_存在するIDなら完了状態がトグルされる()
	{
		var createResponse = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "資料整理" });
		var created = await createResponse.Content.ReadFromJsonAsync<TaskItemDto>();

		var response = await _client.PatchAsync($"/api/tasks/{created!.Id}/complete", null);

		response.EnsureSuccessStatusCode();
		var toggled = await response.Content.ReadFromJsonAsync<TaskItemDto>();
		Assert.True(toggled!.IsCompleted);
	}

	/// <summary>
	/// パス条件: 存在しないIDでPATCH /api/tasks/{id}/completeを呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public async Task ToggleComplete_存在しないIDなら404が返る()
	{
		var response = await _client.PatchAsync("/api/tasks/999999/complete", null);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在するIDでDELETE /api/tasks/{id}を呼ぶと204でタスクが削除されること。
	/// </summary>
	[Fact]
	public async Task Delete_存在するIDなら204で削除される()
	{
		var createResponse = await _client.PostAsJsonAsync("/api/tasks", new TaskItemRequest { Title = "資料整理" });
		var created = await createResponse.Content.ReadFromJsonAsync<TaskItemDto>();

		var response = await _client.DeleteAsync($"/api/tasks/{created!.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		var getResponse = await _client.GetAsync($"/api/tasks/{created.Id}");
		Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
	}

	/// <summary>
	/// パス条件: 存在しないIDでDELETE /api/tasks/{id}を呼ぶと404が返ること。
	/// </summary>
	[Fact]
	public async Task Delete_存在しないIDなら404が返る()
	{
		var response = await _client.DeleteAsync("/api/tasks/999999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}
