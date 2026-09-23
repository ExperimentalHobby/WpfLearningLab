using System.Net;
using System.Text;

namespace SelfHostedNoteSync.Server;

/// <summary>
/// <see cref="NoteApiHandler"/> を <see cref="HttpListener"/> で待ち受けて処理するHTTPサーバー。
/// </summary>
public sealed class NoteHttpServer : IDisposable
{
	private readonly HttpListener _listener = new();
	private readonly NoteApiHandler _handler;
	private CancellationTokenSource? _cts;
	private Task? _acceptLoop;

	/// <summary>
	/// サーバーを初期化する。
	/// </summary>
	/// <param name="handler">リクエストの処理を委譲するハンドラ。</param>
	/// <param name="prefix">待ち受けるURLプレフィックス(例: "http://localhost:5055/")。</param>
	public NoteHttpServer(NoteApiHandler handler, string prefix)
	{
		_handler = handler;
		_listener.Prefixes.Add(prefix);
	}

	/// <summary>
	/// リクエストの受け付けを開始する。
	/// </summary>
	public void Start()
	{
		_listener.Start();
		_cts = new CancellationTokenSource();
		_acceptLoop = AcceptLoopAsync(_cts.Token);
	}

	/// <summary>
	/// リクエストの受け付けを停止し、処理中のループの終了を待つ。
	/// </summary>
	public async Task StopAsync()
	{
		_cts?.Cancel();
		_listener.Stop();

		if (_acceptLoop is not null)
		{
			try
			{
				await _acceptLoop;
			}
			catch (OperationCanceledException)
			{
			}
		}
	}

	private async Task AcceptLoopAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			HttpListenerContext context;
			try
			{
				context = await _listener.GetContextAsync();
			}
			catch (Exception ex) when ((ex is HttpListenerException or ObjectDisposedException) && cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_ = HandleRequestAsync(context);
		}
	}

	private async Task HandleRequestAsync(HttpListenerContext context)
	{
		try
		{
			var response = _handler.Handle(await ReadRequestAsync(context.Request));
			await WriteResponseAsync(context.Response, response);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"リクエスト処理中にエラーが発生しました: {ex}");
			await WriteResponseAsync(context.Response, new ApiResponse(500, null));
		}
	}

	private static async Task<ApiRequest> ReadRequestAsync(HttpListenerRequest request)
	{
		string? body = null;
		if (request.HasEntityBody)
		{
			// Content-Typeにcharsetが無いとContentEncodingが環境依存のコードページになるため、
			// 送信側(HttpClient)の既定に合わせて明示的にUTF-8として読み取る。
			using var reader = new StreamReader(request.InputStream, Encoding.UTF8);
			body = await reader.ReadToEndAsync();
		}

		return new ApiRequest(request.HttpMethod, request.Url?.AbsolutePath ?? "/", body);
	}

	private static async Task WriteResponseAsync(HttpListenerResponse response, ApiResponse apiResponse)
	{
		response.StatusCode = apiResponse.StatusCode;
		response.ContentType = "application/json; charset=utf-8";

		if (apiResponse.Body is not null)
		{
			var buffer = Encoding.UTF8.GetBytes(apiResponse.Body);
			response.ContentLength64 = buffer.Length;
			await response.OutputStream.WriteAsync(buffer);
		}

		response.Close();
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_listener.Close();
		_cts?.Dispose();
	}
}
