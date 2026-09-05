using Markdig;

namespace MarkdownMemo.Services;

/// <summary>
/// Markdigライブラリを使ってMarkdownをHTMLに変換する実装。テーブル等のGFM拡張記法にも対応する。
/// </summary>
public class MarkdigMarkdownToHtmlConverter : IMarkdownToHtmlConverter
{
	private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
		.UseAdvancedExtensions()
		// プレビューはNavigateToStringでHTMLとしてそのまま描画されるため、メモ本文に
		// <script>タグ等の生HTMLを書けてしまうとメモを開くだけでスクリプトが実行される
		// XSSにつながる。DisableHtml()で生HTMLをエスケープしプレーンテキスト扱いにする。
		.DisableHtml()
		.Build();

	/// <inheritdoc/>
	public string Convert(string markdown) => Markdown.ToHtml(markdown, Pipeline);
}
