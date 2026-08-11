namespace RssReader.Models;

/// <summary>
/// RSSフィードの1記事。
/// </summary>
public class RssArticle
{
	/// <summary>記事タイトル。</summary>
	public required string Title { get; init; }

	/// <summary>概要(description)。</summary>
	public string Summary { get; init; } = string.Empty;

	/// <summary>記事へのリンクURL。</summary>
	public string Link { get; init; } = string.Empty;

	/// <summary>公開日時。取得できない場合は<see langword="null"/>。</summary>
	public DateTimeOffset? PublishedDate { get; init; }
}
