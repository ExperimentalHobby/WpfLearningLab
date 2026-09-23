namespace SelfHostedNoteSync.Client.Models;

/// <summary>
/// サーバーから取得するメモ1件を表す。
/// </summary>
public sealed class Note
{
	/// <summary>メモのID。</summary>
	public int Id { get; set; }

	/// <summary>メモのタイトル。</summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>メモの本文。</summary>
	public string Content { get; set; } = string.Empty;
}
