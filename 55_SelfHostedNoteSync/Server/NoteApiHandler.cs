using System.Text.Json;
using SelfHostedNoteSync.Server.Models;

namespace SelfHostedNoteSync.Server;

/// <summary>
/// メモCRUD用のJSON APIリクエストを <see cref="NoteStore"/> に振り分けるルーター。
/// <see cref="System.Net.HttpListener"/> から独立させ、ユニットテスト可能にしている。
/// </summary>
public sealed class NoteApiHandler
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private readonly NoteStore _store;

	/// <summary>
	/// ハンドラを初期化する。
	/// </summary>
	/// <param name="store">操作対象のメモストア。</param>
	public NoteApiHandler(NoteStore store)
	{
		_store = store;
	}

	/// <summary>
	/// リクエストをメソッド・パスで振り分け、対応する処理を実行する。
	/// </summary>
	/// <param name="request">受信したリクエスト。</param>
	/// <returns>返却するレスポンス。</returns>
	public ApiResponse Handle(ApiRequest request)
	{
		var segments = request.Path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

		if (segments.Length == 1 && segments[0] == "notes")
		{
			return request.Method switch
			{
				"GET" => HandleGetAll(),
				"POST" => HandlePost(request.Body),
				_ => NotFound(),
			};
		}

		if (segments.Length == 2 && segments[0] == "notes" && int.TryParse(segments[1], out var id))
		{
			return request.Method switch
			{
				"GET" => HandleGetById(id),
				"PUT" => HandlePut(id, request.Body),
				"DELETE" => HandleDelete(id),
				_ => NotFound(),
			};
		}

		return NotFound();
	}

	private ApiResponse HandleGetAll()
	{
		var json = JsonSerializer.Serialize(_store.GetAll(), JsonOptions);
		return new ApiResponse(200, json);
	}

	private ApiResponse HandleGetById(int id)
	{
		var note = _store.GetById(id);
		if (note is null)
		{
			return NotFound();
		}

		return new ApiResponse(200, JsonSerializer.Serialize(note, JsonOptions));
	}

	private ApiResponse HandlePost(string? body)
	{
		if (!TryParseInput(body, out var input))
		{
			return BadRequest();
		}

		if (string.IsNullOrWhiteSpace(input.Title))
		{
			return BadRequest();
		}

		var created = _store.Add(new Note { Title = input.Title, Content = input.Content });
		return new ApiResponse(201, JsonSerializer.Serialize(created, JsonOptions));
	}

	private ApiResponse HandlePut(int id, string? body)
	{
		if (!TryParseInput(body, out var input))
		{
			return BadRequest();
		}

		if (string.IsNullOrWhiteSpace(input.Title))
		{
			return BadRequest();
		}

		var note = new Note { Title = input.Title, Content = input.Content };
		if (!_store.Update(id, note))
		{
			return NotFound();
		}

		return new ApiResponse(200, JsonSerializer.Serialize(note, JsonOptions));
	}

	private ApiResponse HandleDelete(int id)
	{
		return _store.Delete(id) ? new ApiResponse(204, null) : NotFound();
	}

	private static bool TryParseInput(string? body, out NoteInput input)
	{
		if (string.IsNullOrWhiteSpace(body))
		{
			input = new NoteInput();
			return false;
		}

		try
		{
			var parsed = JsonSerializer.Deserialize<NoteInput>(body, JsonOptions);
			if (parsed is null)
			{
				input = new NoteInput();
				return false;
			}

			input = parsed;
			return true;
		}
		catch (JsonException)
		{
			input = new NoteInput();
			return false;
		}
	}

	private static ApiResponse NotFound() => new(404, null);

	private static ApiResponse BadRequest() => new(400, null);
}
