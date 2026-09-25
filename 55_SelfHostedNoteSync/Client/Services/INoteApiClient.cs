using SelfHostedNoteSync.Client.Models;

namespace SelfHostedNoteSync.Client.Services;

/// <summary>
/// メモ同期サーバーとの通信を担うクライアントの抽象。
/// </summary>
public interface INoteApiClient
{
	/// <summary>
	/// メモの一覧を取得する。
	/// </summary>
	Task<IReadOnlyList<Note>> GetNotesAsync();

	/// <summary>
	/// メモを新規作成する。
	/// </summary>
	Task<Note> CreateNoteAsync(string title, string content);

	/// <summary>
	/// 指定IDのメモを更新する。
	/// </summary>
	Task<Note> UpdateNoteAsync(int id, string title, string content);

	/// <summary>
	/// 指定IDのメモを削除する。
	/// </summary>
	Task DeleteNoteAsync(int id);
}
