using RssReader.Models;

namespace RssReader.Services;

/// <summary>
/// RSSフィードの取得を担うクライアントの抽象。
/// </summary>
public interface IRssFeedClient
{
	/// <summary>
	/// 指定したURLからRSSフィードを取得し、記事一覧を返す。
	/// </summary>
	/// <param name="feedUrl">RSSフィードのURL。</param>
	Task<IReadOnlyList<RssArticle>> FetchAsync(string feedUrl);
}
