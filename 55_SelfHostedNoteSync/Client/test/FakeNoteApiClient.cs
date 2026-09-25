using SelfHostedNoteSync.Client.Models;
using SelfHostedNoteSync.Client.Services;

namespace SelfHostedNoteSync.Client.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実通信を行わない<see cref="INoteApiClient"/>実装。
/// 各メソッドの戻り値・例外・呼び出し引数を差し替え/検証可能にする。
/// </summary>
public class FakeNoteApiClient : INoteApiClient
{
	public IReadOnlyList<Note> NotesResult { get; set; } = [];
	public Note? CreateResult { get; set; }
	public Note? UpdateResult { get; set; }
	public Exception? ExceptionToThrow { get; set; }

	public (string Title, string Content)? LastCreateCall { get; private set; }
	public (int Id, string Title, string Content)? LastUpdateCall { get; private set; }
	public int? LastDeletedId { get; private set; }

	public Task<IReadOnlyList<Note>> GetNotesAsync()
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(NotesResult);
	}

	public Task<Note> CreateNoteAsync(string title, string content)
	{
		LastCreateCall = (title, content);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(CreateResult!);
	}

	public Task<Note> UpdateNoteAsync(int id, string title, string content)
	{
		LastUpdateCall = (id, title, content);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(UpdateResult!);
	}

	public Task DeleteNoteAsync(int id)
	{
		LastDeletedId = id;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.CompletedTask;
	}
}
