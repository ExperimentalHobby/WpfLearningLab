using Markdig;

namespace MarkdownMemo.Services;

/// <summary>
/// Markdigライブラリを使ってMarkdownをHTMLに変換する実装。テーブル等のGFM拡張記法にも対応する。
/// </summary>
public class MarkdigMarkdownToHtmlConverter : IMarkdownToHtmlConverter
{
	private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
		.UseAdvancedExtensions()
		.Build();

	/// <inheritdoc/>
	public string Convert(string markdown) => Markdown.ToHtml(markdown, Pipeline);
}
