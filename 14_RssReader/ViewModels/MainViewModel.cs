using System.Collections.ObjectModel;
using System.Net.Http;
using System.Xml;
using RssReader.Models;
using RssReader.Services;

namespace RssReader.ViewModels;

/// <summary>
/// RSSリーダーのメイン画面のViewModel。フィードの取得・記事一覧表示・エラー処理を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IRssFeedClient _feedClient;
	private readonly IBrowserLauncher _browserLauncher;

	private string _feedUrl = string.Empty;
	private bool _isLoading;
	private string _errorMessage = string.Empty;
	private RssArticle? _selectedArticle;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="feedClient">RSSフィード取得に使うクライアント。</param>
	/// <param name="browserLauncher">リンクを開くブラウザ起動処理。</param>
	public MainViewModel(IRssFeedClient feedClient, IBrowserLauncher browserLauncher)
	{
		_feedClient = feedClient;
		_browserLauncher = browserLauncher;
		FetchCommand = new AsyncRelayCommand(FetchAsync, CanFetch);
		OpenLinkCommand = new RelayCommand<string>(OpenLink, CanOpenLink);
	}

	/// <summary>取得するRSSフィードのURL。</summary>
	public string FeedUrl
	{
		get => _feedUrl;
		set
		{
			if (SetProperty(ref _feedUrl, value))
			{
				FetchCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>取得した記事一覧。</summary>
	public ObservableCollection<RssArticle> Articles { get; } = [];

	/// <summary>通信中かどうか。</summary>
	public bool IsLoading
	{
		get => _isLoading;
		private set => SetProperty(ref _isLoading, value);
	}

	/// <summary>エラーメッセージ。エラーが無い場合は空文字。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>選択中の記事。</summary>
	public RssArticle? SelectedArticle
	{
		get => _selectedArticle;
		set
		{
			if (SetProperty(ref _selectedArticle, value))
			{
				OpenLinkCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>
	/// <see cref="FeedUrl"/>のRSSフィードを取得するコマンド。
	/// </summary>
	public AsyncRelayCommand FetchCommand { get; }

	/// <summary>
	/// 記事のリンクを既定のブラウザで開くコマンド。パラメータはURL。
	/// </summary>
	public RelayCommand<string> OpenLinkCommand { get; }

	private bool CanFetch() => !string.IsNullOrWhiteSpace(FeedUrl);

	private bool CanOpenLink(string? url) => !string.IsNullOrWhiteSpace(url);

	private void OpenLink(string? url)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			return;
		}

		if (!_browserLauncher.Open(url))
		{
			ErrorMessage = "安全でないリンクのため開けませんでした。";
		}
	}

	private async Task FetchAsync()
	{
		IsLoading = true;
		ErrorMessage = string.Empty;
		try
		{
			var articles = await _feedClient.FetchAsync(FeedUrl);
			Articles.Clear();
			foreach (var article in articles)
			{
				Articles.Add(article);
			}
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or XmlException or UriFormatException)
		{
			ErrorMessage = "フィードの取得に失敗しました。URLと通信環境を確認してください。";
		}
		finally
		{
			IsLoading = false;
		}
	}
}
