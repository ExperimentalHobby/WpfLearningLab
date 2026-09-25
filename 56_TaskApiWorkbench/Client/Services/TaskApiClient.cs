using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TaskApiWorkbench.Client.Models;

namespace TaskApiWorkbench.Client.Services;

/// <summary>
/// 56_TaskApiWorkbench/Server(ASP.NET Core Minimal API)と通信するクライアント。
/// </summary>
public class TaskApiClient : ITaskApiClient
{
	private const string BaseUrl = "http://localhost:5100/api/tasks";

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private readonly HttpClient _httpClient;

	/// <summary>
	/// クライアントを初期化する。
	/// </summary>
	/// <param name="httpClient">API呼び出しに使う<see cref="HttpClient"/>。テスト時は差し替え可能。</param>
	public TaskApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <inheritdoc/>
	public async Task<IReadOnlyList<TaskItem>> GetTasksAsync()
	{
		using var stream = await _httpClient.GetStreamAsync(BaseUrl);
		var tasks = await JsonSerializer.DeserializeAsync<List<TaskItem>>(stream, JsonOptions);
		return tasks ?? [];
	}

	/// <inheritdoc/>
	public async Task<TaskItem> CreateTaskAsync(string title, string description)
	{
		var input = new TaskItemRequest { Title = title, Description = description };
		using var response = await _httpClient.PostAsJsonAsync(BaseUrl, input, JsonOptions);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TaskItem>(JsonOptions)
			?? throw new InvalidOperationException("サーバーからの応答を解析できませんでした。");
	}

	/// <inheritdoc/>
	public async Task<TaskItem> UpdateTaskAsync(int id, string title, string description)
	{
		var input = new TaskItemRequest { Title = title, Description = description };
		using var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", input, JsonOptions);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TaskItem>(JsonOptions)
			?? throw new InvalidOperationException("サーバーからの応答を解析できませんでした。");
	}

	/// <inheritdoc/>
	public async Task<TaskItem> ToggleCompleteAsync(int id)
	{
		using var response = await _httpClient.PatchAsync($"{BaseUrl}/{id}/complete", null);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<TaskItem>(JsonOptions)
			?? throw new InvalidOperationException("サーバーからの応答を解析できませんでした。");
	}

	/// <inheritdoc/>
	public async Task DeleteTaskAsync(int id)
	{
		using var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
		response.EnsureSuccessStatusCode();
	}
}
