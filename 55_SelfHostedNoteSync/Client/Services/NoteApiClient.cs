using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using SelfHostedNoteSync.Client.Models;

namespace SelfHostedNoteSync.Client.Services;

/// <summary>
/// 55_SelfHostedNoteSync/Server(HttpListener自作サーバー)と通信するクライアント。
/// </summary>
public class NoteApiClient : INoteApiClient
{
	private const string BaseUrl = "http://localhost:5055/notes";

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private readonly HttpClient _httpClient;

	/// <summary>
	/// クライアントを初期化する。
	/// </summary>
	/// <param name="httpClient">API呼び出しに使う<see cref="HttpClient"/>。テスト時は差し替え可能。</param>
	public NoteApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <inheritdoc/>
	public async Task<IReadOnlyList<Note>> GetNotesAsync()
	{
		using var stream = await _httpClient.GetStreamAsync(BaseUrl);
		var notes = await JsonSerializer.DeserializeAsync<List<Note>>(stream, JsonOptions);
		return notes ?? [];
	}

	/// <inheritdoc/>
	public async Task<Note> CreateNoteAsync(string title, string content)
	{
		var input = new NoteInput { Title = title, Content = content };
		using var response = await _httpClient.PostAsJsonAsync(BaseUrl, input, JsonOptions);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<Note>(JsonOptions)
			?? throw new InvalidOperationException("サーバーからの応答を解析できませんでした。");
	}

	/// <inheritdoc/>
	public async Task<Note> UpdateNoteAsync(int id, string title, string content)
	{
		var input = new NoteInput { Title = title, Content = content };
		using var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", input, JsonOptions);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<Note>(JsonOptions)
			?? throw new InvalidOperationException("サーバーからの応答を解析できませんでした。");
	}

	/// <inheritdoc/>
	public async Task DeleteNoteAsync(int id)
	{
		using var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
		response.EnsureSuccessStatusCode();
	}
}
