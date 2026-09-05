using RssReader.Models;
using RssReader.ViewModels;

namespace RssReader.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(FakeRssFeedClient? feedClient = null, FakeBrowserLauncher? browserLauncher = null) =>
		new(feedClient ?? new FakeRssFeedClient(), browserLauncher ?? new FakeBrowserLauncher());

	/// <summary>
	/// パス条件: FetchCommand実行でArticlesにフィード結果が反映されること
	/// </summary>
	[Fact]
	public async Task FetchCommand_実行するとArticlesにフィード結果が反映される()
	{
		var feedClient = new FakeRssFeedClient
		{
			ArticlesToReturn = [new RssArticle { Title = "記事1", Link = "https://example.com/1" }],
		};
		var viewModel = CreateViewModel(feedClient);
		viewModel.FeedUrl = "https://example.com/rss";

		viewModel.FetchCommand.Execute(null);
		await Task.Delay(50);

		var article = Assert.Single(viewModel.Articles);
		Assert.Equal("記事1", article.Title);
	}

	/// <summary>
	/// パス条件: 取得中はIsLoadingがtrueになり、完了後falseに戻ること
	/// </summary>
	[Fact]
	public async Task FetchCommand_取得中はIsLoadingがtrueになり完了後falseに戻る()
	{
		var gate = new TaskCompletionSource();
		var feedClient = new FakeRssFeedClient { Gate = gate };
		var viewModel = CreateViewModel(feedClient);
		viewModel.FeedUrl = "https://example.com/rss";

		viewModel.FetchCommand.Execute(null);
		var isLoadingDuring = viewModel.IsLoading;
		gate.SetResult();
		await Task.Delay(50);

		Assert.True(isLoadingDuring);
		Assert.False(viewModel.IsLoading);
	}

	/// <summary>
	/// パス条件: 取得失敗時(例外発生)にErrorMessageが設定されること
	/// </summary>
	[Fact]
	public async Task FetchCommand_取得失敗時にErrorMessageが設定される()
	{
		var feedClient = new FakeRssFeedClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = CreateViewModel(feedClient);
		viewModel.FeedUrl = "https://example.com/rss";

		viewModel.FetchCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 再取得時に前回のErrorMessageがクリアされること
	/// </summary>
	[Fact]
	public async Task FetchCommand_再取得時に前回のErrorMessageがクリアされる()
	{
		var feedClient = new FakeRssFeedClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = CreateViewModel(feedClient);
		viewModel.FeedUrl = "https://example.com/rss";
		viewModel.FetchCommand.Execute(null);
		await Task.Delay(50);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);

		feedClient.ExceptionToThrow = null;
		feedClient.ArticlesToReturn = [new RssArticle { Title = "記事1" }];

		viewModel.FetchCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: FeedUrlが空欄の場合、FetchCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void FetchCommand_FeedUrlが空欄の場合CanExecuteがfalseになる(string feedUrl)
	{
		var viewModel = CreateViewModel();
		viewModel.FeedUrl = feedUrl;

		var canExecute = viewModel.FetchCommand.CanExecute(null);

		Assert.False(canExecute);
	}

	/// <summary>
	/// パス条件: OpenLinkCommand実行で、IBrowserLauncherに正しいURLが渡されること
	/// </summary>
	[Fact]
	public void OpenLinkCommand_実行するとBrowserLauncherに正しいURLが渡される()
	{
		var browserLauncher = new FakeBrowserLauncher();
		var viewModel = CreateViewModel(browserLauncher: browserLauncher);

		viewModel.OpenLinkCommand.Execute("https://example.com/1");

		Assert.Equal("https://example.com/1", browserLauncher.LastOpenedUrl);
	}

	/// <summary>
	/// パス条件: BrowserLauncherが安全でないURLとして拒否(falseを返す)した場合、
	/// ErrorMessageが設定されること
	/// </summary>
	[Fact]
	public void OpenLinkCommand_BrowserLauncherが拒否した場合ErrorMessageが設定される()
	{
		var browserLauncher = new FakeBrowserLauncher { ReturnValue = false };
		var viewModel = CreateViewModel(browserLauncher: browserLauncher);

		viewModel.OpenLinkCommand.Execute("file:///C:/Windows/System32/calc.exe");

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: リンクがnullまたは空の場合、OpenLinkCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void OpenLinkCommand_リンクが無い場合CanExecuteがfalseになる(string? link)
	{
		var viewModel = CreateViewModel();

		var canExecute = viewModel.OpenLinkCommand.CanExecute(link);

		Assert.False(canExecute);
	}

	/// <summary>
	/// パス条件: FeedUrlを変更すると、FetchCommandのCanExecuteChangedが発火し、ボタンの有効/無効がUIに追従すること
	/// </summary>
	[Fact]
	public void FeedUrl_変更するとFetchCommandのCanExecuteChangedが発火する()
	{
		var viewModel = CreateViewModel();
		var raised = false;
		viewModel.FetchCommand.CanExecuteChanged += (_, _) => raised = true;

		viewModel.FeedUrl = "https://example.com/rss";

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: SelectedArticleを変更すると、OpenLinkCommandのCanExecuteChangedが発火すること
	/// </summary>
	[Fact]
	public void SelectedArticle_変更するとOpenLinkCommandのCanExecuteChangedが発火する()
	{
		var viewModel = CreateViewModel();
		var raised = false;
		viewModel.OpenLinkCommand.CanExecuteChanged += (_, _) => raised = true;

		viewModel.SelectedArticle = new RssArticle { Title = "記事1", Link = "https://example.com/1" };

		Assert.True(raised);
	}
}
