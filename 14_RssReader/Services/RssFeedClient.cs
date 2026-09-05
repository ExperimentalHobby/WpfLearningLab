using System.Net.Http;
using System.Xml.Linq;
using RssReader.Models;

namespace RssReader.Services;

/// <summary>
/// RSS 2.0形式のフィードをHttpClientで取得し、<see cref="XDocument"/>でパースするクライアント。
/// </summary>
public class RssFeedClient : IRssFeedClient
{
	private readonly HttpClient _httpClient;

	/// <summary>
	/// クライアントを初期化する。
	/// </summary>
	/// <param name="httpClient">フィード取得に使う<see cref="HttpClient"/>。テスト時は差し替え可能。</param>
	public RssFeedClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <inheritdoc/>
	public async Task<IReadOnlyList<RssArticle>> FetchAsync(string feedUrl)
	{
		using var stream = await _httpClient.GetStreamAsync(feedUrl);
		var document = await Task.Run(() => XDocument.Load(stream));

		// RSS 2.0の<item>とAtomの<entry>の両方に対応する。要素名の一致は名前空間を
		// 無視したLocalNameで行う。RSS 2.0では名前空間の宣言は本来不要だが、実際には
		// <rss>/<channel>に既定の名前空間を宣言しているフィードも存在し、その場合は
		// XName完全一致(名前空間なし)の検索では配下の要素が一致しなくなるため。
		var entries = document.Descendants().Where(e => e.Name.LocalName is "item" or "entry");

		var articles = new List<RssArticle>();
		foreach (var entry in entries)
		{
			var title = GetChildValue(entry, "title") ?? string.Empty;
			// summary/contentはAtomでの別名。
			var summary = GetChildValue(entry, "description")
				?? GetChildValue(entry, "summary")
				?? GetChildValue(entry, "content")
				?? string.Empty;
			var link = GetLink(entry);
			// published/updatedはAtomでの別名。
			var pubDateText = GetChildValue(entry, "pubDate")
				?? GetChildValue(entry, "published")
				?? GetChildValue(entry, "updated");
			DateTimeOffset? publishedDate = DateTimeOffset.TryParse(pubDateText, out var parsed) ? parsed : null;

			articles.Add(new RssArticle
			{
				Title = title,
				Summary = summary,
				Link = link,
				PublishedDate = publishedDate,
			});
		}

		return articles;
	}

	/// <summary>
	/// 名前空間を無視して、指定したローカル名の直下の子要素の値を取得する。
	/// </summary>
	private static string? GetChildValue(XElement parent, string localName) =>
		parent.Elements().FirstOrDefault(e => e.Name.LocalName == localName)?.Value;

	/// <summary>
	/// link要素からリンクURLを取得する。RSSはテキストの値としてURLを持つが、
	/// Atomは<c>&lt;link href="..."/&gt;</c>のように href 属性にURLを持つため、両方に対応する。
	/// </summary>
	private static string GetLink(XElement entry)
	{
		var linkElement = entry.Elements().FirstOrDefault(e => e.Name.LocalName == "link");
		if (linkElement is null)
		{
			return string.Empty;
		}

		return !string.IsNullOrEmpty(linkElement.Value)
			? linkElement.Value
			: linkElement.Attribute("href")?.Value ?? string.Empty;
	}
}
