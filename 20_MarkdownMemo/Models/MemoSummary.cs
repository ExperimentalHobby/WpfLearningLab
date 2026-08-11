namespace MarkdownMemo.Models;

/// <summary>
/// メモ一覧表示用のサマリ。タイトルがメモの識別子(ファイル名)を兼ねる。
/// </summary>
/// <param name="Title">メモのタイトル(識別子)。</param>
/// <param name="LastModified">最終更新日時。</param>
public record MemoSummary(string Title, DateTime LastModified);
