using SelfHostedNoteSync.Client.Models;
using SelfHostedNoteSync.Client.Services;

namespace SelfHostedNoteSync.Client.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実通信を行わない<see cref="INoteApiClient"/>実装。
/// 各メソッドの戻り値・例外・呼び出し引数を差し替え/検証可能にする。
/// </summary>
public class FakeNoteApiClient : INoteApiClient
{
	/// <summary><see cref="GetNotesAsync"/>が返す値(テスト用)。</summary>
	public IReadOnlyList<Note> NotesResult { get; set; } = [];

	/// <summary><see cref="CreateNoteAsync"/>が返す値(テスト用)。</summary>
	public Note? CreateResult { get; set; }

	/// <summary><see cref="UpdateNoteAsync"/>が返す値(テスト用)。</summary>
	public Note? UpdateResult { get; set; }

	/// <summary>設定すると各メソッド呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <summary>直近の<see cref="CreateNoteAsync"/>呼び出し引数(テスト用)。</summary>
	public (string Title, string Content)? LastCreateCall { get; private set; }

	/// <summary>直近の<see cref="UpdateNoteAsync"/>呼び出し引数(テスト用)。</summary>
	public (int Id, string Title, string Content)? LastUpdateCall { get; private set; }

	/// <summary>直近の<see cref="DeleteNoteAsync"/>呼び出し引数(テスト用)。</summary>
	public int? LastDeletedId { get; private set; }

	/// <inheritdoc/>
	public Task<IReadOnlyList<Note>> GetNotesAsync()
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(NotesResult);
	}

	/// <inheritdoc/>
	public Task<Note> CreateNoteAsync(string title, string content)
	{
		LastCreateCall = (title, content);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(CreateResult!);
	}

	/// <inheritdoc/>
	public Task<Note> UpdateNoteAsync(int id, string title, string content)
	{
		LastUpdateCall = (id, title, content);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(UpdateResult!);
	}

	/// <inheritdoc/>
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
