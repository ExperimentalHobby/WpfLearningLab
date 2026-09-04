namespace AccessibleNoteApp.Models;

/// <summary>
/// メモ1件を表す。
/// </summary>
/// <param name="Id">一意なID(ファイル名にも使う)。</param>
/// <param name="Title">タイトル。</param>
/// <param name="Body">本文。</param>
/// <param name="UpdatedAt">最終更新日時。</param>
public sealed record Memo(string Id, string Title, string Body, DateTime UpdatedAt);
