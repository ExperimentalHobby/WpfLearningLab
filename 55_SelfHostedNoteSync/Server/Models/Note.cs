namespace SelfHostedNoteSync.Server.Models;

/// <summary>
/// サーバーが保持するメモ1件を表す。
/// </summary>
public sealed class Note
{
	/// <summary>
	/// メモのID。<see cref="NoteStore"/> が採番する。
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// メモのタイトル。
	/// </summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// メモの本文。
	/// </summary>
	public string Content { get; set; } = string.Empty;
}
