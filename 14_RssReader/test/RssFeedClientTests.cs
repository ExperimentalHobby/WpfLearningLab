using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="RssFeedClient"/> の単体テスト。
/// 実ネットワーク通信はせず、<see cref="FakeHttpMessageHandler"/>でXML応答を差し替える。
/// </summary>
public class RssFeedClientTests
{
	/// <summary>
	/// パス条件: 正常なRSS XMLから記事一覧(タイトル・概要・リンク・日時)を取得できること
	/// </summary>
	[Fact]
	public async Task FetchAsync_正常なRSSから記事一覧を取得できる()
	{
		const string xml = """
			<?xml version="1.0" encoding="UTF-8"?>
			<rss version="2.0">
			  <channel>
			    <title>サンプルフィード</title>
			    <item>
			      <title>記事1</title>
			      <description>記事1の概要</description>
			      <link>https://example.com/1</link>
			      <pubDate>Tue, 11 Aug 2026 09:00:00 GMT</pubDate>
			    </item>
			  </channel>
			</rss>
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(xml));
		var client = new RssFeedClient(httpClient);

		var articles = await client.FetchAsync("https://example.com/rss");

		var article = Assert.Single(articles);
		Assert.Equal("記事1", article.Title);
		Assert.Equal("記事1の概要", article.Summary);
		Assert.Equal("https://example.com/1", article.Link);
		Assert.NotNull(article.PublishedDate);
	}

	/// <summary>
	/// パス条件: pubDate要素が無い項目でも例外にならず、PublishedDateがnullになること
	/// </summary>
	[Fact]
	public async Task FetchAsync_pubDateが無い項目でも例外にならない()
	{
		const string xml = """
			<?xml version="1.0" encoding="UTF-8"?>
			<rss version="2.0">
			  <channel>
			    <title>サンプルフィード</title>
			    <item>
			      <title>記事1</title>
			      <description>記事1の概要</description>
			      <link>https://example.com/1</link>
			    </item>
			  </channel>
			</rss>
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(xml));
		var client = new RssFeedClient(httpClient);

		var articles = await client.FetchAsync("https://example.com/rss");

		var article = Assert.Single(articles);
		Assert.Null(article.PublishedDate);
	}

	/// <summary>
	/// パス条件: item要素が無いfeedの場合、空のリストを返すこと
	/// </summary>
	[Fact]
	public async Task FetchAsync_itemが無い場合空リストを返す()
	{
		const string xml = """
			<?xml version="1.0" encoding="UTF-8"?>
			<rss version="2.0">
			  <channel>
			    <title>サンプルフィード</title>
			  </channel>
			</rss>
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(xml));
		var client = new RssFeedClient(httpClient);

		var articles = await client.FetchAsync("https://example.com/rss");

		Assert.Empty(articles);
	}
}
