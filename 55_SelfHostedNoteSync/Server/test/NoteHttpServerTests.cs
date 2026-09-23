using System.Net.Http.Json;
using System.Text.Json;
using SelfHostedNoteSync.Server;
using SelfHostedNoteSync.Server.Models;

namespace SelfHostedNoteSync.Server.Tests;

/// <summary>
/// <see cref="NoteHttpServer"/> の結合テスト。
/// 実際に<see cref="System.Net.HttpListener"/>を起動し、HttpClientで通信してエンドツーエンドの挙動を検証する。
/// </summary>
public class NoteHttpServerTests
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	/// <summary>
	/// パス条件: 日本語を含むメモをPOST/GETすると、文字化けせずに往復できること。
	/// </summary>
	[Fact]
	public async Task 日本語を含むメモが文字化けせずに往復できる()
	{
		var prefix = $"http://localhost:{GetFreePort()}/";
		using var server = new NoteHttpServer(new NoteApiHandler(new NoteStore()), prefix);
		server.Start();
		using var httpClient = new HttpClient();

		var createResponse = await httpClient.PostAsJsonAsync($"{prefix}notes", new NoteInput { Title = "買い物", Content = "牛乳と卵" }, JsonOptions);
		createResponse.EnsureSuccessStatusCode();
		var created = await createResponse.Content.ReadFromJsonAsync<Note>(JsonOptions);

		var listResponse = await httpClient.GetAsync($"{prefix}notes");
		listResponse.EnsureSuccessStatusCode();
		var notes = await listResponse.Content.ReadFromJsonAsync<List<Note>>(JsonOptions);

		await server.StopAsync();

		Assert.Equal("買い物", created!.Title);
		Assert.Equal("牛乳と卵", created.Content);
		Assert.Equal("買い物", notes![0].Title);
	}

	private static int GetFreePort()
	{
		using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
		listener.Start();
		var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();
		return port;
	}
}
