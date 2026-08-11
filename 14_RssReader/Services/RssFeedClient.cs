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

		var items = document.Descendants("item");
		var articles = new List<RssArticle>();
		foreach (var item in items)
		{
			var title = item.Element("title")?.Value ?? string.Empty;
			var summary = item.Element("description")?.Value ?? string.Empty;
			var link = item.Element("link")?.Value ?? string.Empty;
			var pubDateText = item.Element("pubDate")?.Value;
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
}
