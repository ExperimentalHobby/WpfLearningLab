namespace SelfHostedNoteSync.Server.Models;

/// <summary>
/// メモの作成・更新リクエストのボディを表す。
/// </summary>
public sealed class NoteInput
{
	/// <summary>
	/// メモのタイトル。
	/// </summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// メモの本文。
	/// </summary>
	public string Content { get; set; } = string.Empty;
}
