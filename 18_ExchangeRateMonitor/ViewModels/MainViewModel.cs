using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using ExchangeRateMonitor.Services;

namespace ExchangeRateMonitor.ViewModels;

/// <summary>
/// 為替モニターアプリのメイン画面のViewModel。通貨ペアの登録・削除と一括更新(リトライ込み)を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	/// <summary>1銘柄あたりの取得リトライ回数(初回含む)。</summary>
	private const int MaxAttempts = 3;

	private readonly IExchangeRateApiClient _apiClient;
	private readonly TimeSpan _retryDelay;

	private string _inputBaseCurrency = string.Empty;
	private string _inputQuoteCurrency = string.Empty;
	private DateTime? _lastUpdated;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="apiClient">為替レート取得に使うクライアント。</param>
	/// <param name="retryDelay">取得失敗時のリトライ間隔(省略時は1秒)。テストでは<see cref="TimeSpan.Zero"/>を指定して高速化する。</param>
	public MainViewModel(IExchangeRateApiClient apiClient, TimeSpan? retryDelay = null)
	{
		_apiClient = apiClient;
		_retryDelay = retryDelay ?? TimeSpan.FromSeconds(1);

		AddPairCommand = new RelayCommand(AddPair, CanAddPair);
		RemovePairCommand = new RelayCommand<WatchedPairItem>(RemovePair);
		RefreshAllCommand = new AsyncRelayCommand(RefreshAllAsync);
	}

	/// <summary>監視中の通貨ペア一覧。</summary>
	public ObservableCollection<WatchedPairItem> WatchedPairs { get; } = [];

	/// <summary>入力フォーム: 基軸通貨コード(例: "USD")。</summary>
	public string InputBaseCurrency
	{
		get => _inputBaseCurrency;
		set
		{
			if (SetProperty(ref _inputBaseCurrency, value))
			{
				AddPairCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>入力フォーム: 決済通貨コード(例: "JPY")。</summary>
	public string InputQuoteCurrency
	{
		get => _inputQuoteCurrency;
		set
		{
			if (SetProperty(ref _inputQuoteCurrency, value))
			{
				AddPairCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>直近の一括更新が完了した日時。未更新の場合は<see langword="null"/>。</summary>
	public DateTime? LastUpdated
	{
		get => _lastUpdated;
		private set => SetProperty(ref _lastUpdated, value);
	}

	/// <summary>入力フォームの通貨ペアを監視銘柄一覧に追加するコマンド。</summary>
	public RelayCommand AddPairCommand { get; }

	/// <summary>指定した監視銘柄を一覧から削除するコマンド。</summary>
	public RelayCommand<WatchedPairItem> RemovePairCommand { get; }

	/// <summary>登録済み全銘柄のレートを一括取得(リトライ込み)するコマンド。</summary>
	public AsyncRelayCommand RefreshAllCommand { get; }

	private static bool IsValidCurrencyCode(string code) =>
		code.Length == 3 && code.All(char.IsAsciiLetter);

	private bool CanAddPair() =>
		IsValidCurrencyCode(InputBaseCurrency.Trim()) &&
		IsValidCurrencyCode(InputQuoteCurrency.Trim()) &&
		!WatchedPairs.Any(pair =>
			string.Equals(pair.BaseCurrency, InputBaseCurrency.Trim(), StringComparison.OrdinalIgnoreCase) &&
			string.Equals(pair.QuoteCurrency, InputQuoteCurrency.Trim(), StringComparison.OrdinalIgnoreCase));

	private void AddPair()
	{
		var baseCurrency = InputBaseCurrency.Trim().ToUpperInvariant();
		var quoteCurrency = InputQuoteCurrency.Trim().ToUpperInvariant();
		WatchedPairs.Add(new WatchedPairItem(baseCurrency, quoteCurrency));

		InputBaseCurrency = string.Empty;
		InputQuoteCurrency = string.Empty;
	}

	private void RemovePair(WatchedPairItem? item)
	{
		if (item is null)
		{
			return;
		}

		WatchedPairs.Remove(item);
	}

	private async Task RefreshAllAsync()
	{
		foreach (var pair in WatchedPairs.ToList())
		{
			await RefreshOneAsync(pair);
		}

		LastUpdated = DateTime.Now;
	}

	private async Task RefreshOneAsync(WatchedPairItem pair)
	{
		pair.IsLoading = true;
		try
		{
			for (var attempt = 1; attempt <= MaxAttempts; attempt++)
			{
				try
				{
					var rate = await _apiClient.GetRateAsync(pair.BaseCurrency, pair.QuoteCurrency);
					pair.UpdateRate(rate);
					return;
				}
				catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException
					or InvalidOperationException or FormatException)
				{
					if (attempt >= MaxAttempts)
					{
						pair.ErrorMessage = $"{pair.DisplayName} の取得に失敗しました。";
						return;
					}

					await Task.Delay(_retryDelay);
				}
			}
		}
		finally
		{
			pair.IsLoading = false;
		}
	}
}
