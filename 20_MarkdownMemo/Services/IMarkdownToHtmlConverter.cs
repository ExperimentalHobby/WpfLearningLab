namespace MarkdownMemo.Services;

/// <summary>
/// Markdown文字列をHTML文字列に変換する処理の抽象。
/// </summary>
public interface IMarkdownToHtmlConverter
{
	/// <summary>
	/// Markdown文字列をHTMLに変換する。
	/// </summary>
	/// <param name="markdown">変換元のMarkdown文字列。</param>
	string Convert(string markdown);
}
