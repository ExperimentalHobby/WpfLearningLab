namespace ExchangeRateMonitor.ViewModels;

/// <summary>
/// 一覧表示用の監視通貨ペア。取得済みレートと前回比較したトレンドを保持する。
/// </summary>
public class WatchedPairItem : ObservableObject
{
	private decimal? _currentRate;
	private decimal? _previousRate;
	private RateTrend _trend = RateTrend.Unknown;
	private string _errorMessage = string.Empty;
	private bool _isLoading;

	/// <summary>
	/// 監視通貨ペアを初期化する。
	/// </summary>
	/// <param name="baseCurrency">基軸通貨コード(例: "USD")。</param>
	/// <param name="quoteCurrency">決済通貨コード(例: "JPY")。</param>
	public WatchedPairItem(string baseCurrency, string quoteCurrency)
	{
		BaseCurrency = baseCurrency;
		QuoteCurrency = quoteCurrency;
	}

	/// <summary>基軸通貨コード。</summary>
	public string BaseCurrency { get; }

	/// <summary>決済通貨コード。</summary>
	public string QuoteCurrency { get; }

	/// <summary>表示名(例: "USD/JPY")。</summary>
	public string DisplayName => $"{BaseCurrency}/{QuoteCurrency}";

	/// <summary>直近取得したレート。未取得の場合は<see langword="null"/>。</summary>
	public decimal? CurrentRate
	{
		get => _currentRate;
		private set => SetProperty(ref _currentRate, value);
	}

	/// <summary>1つ前に取得したレート。前回比較の表示に使う。</summary>
	public decimal? PreviousRate
	{
		get => _previousRate;
		private set => SetProperty(ref _previousRate, value);
	}

	/// <summary>前回レートと比較した変動方向。</summary>
	public RateTrend Trend
	{
		get => _trend;
		private set => SetProperty(ref _trend, value);
	}

	/// <summary>直近の取得でエラーが発生した場合のメッセージ。エラーが無い場合は空文字。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>取得中かどうか。</summary>
	public bool IsLoading
	{
		get => _isLoading;
		set => SetProperty(ref _isLoading, value);
	}

	/// <summary>
	/// 新しいレートで更新する。直前のレートとの比較で<see cref="Trend"/>を判定し、
	/// <see cref="ErrorMessage"/>をクリアする。
	/// </summary>
	/// <param name="newRate">新しく取得したレート。</param>
	public void UpdateRate(decimal newRate)
	{
		var previous = CurrentRate;
		PreviousRate = previous;
		CurrentRate = newRate;
		Trend = previous is null
			? RateTrend.Unknown
			: newRate > previous.Value
				? RateTrend.Up
				: newRate < previous.Value
					? RateTrend.Down
					: RateTrend.Unchanged;
		ErrorMessage = string.Empty;
	}
}
