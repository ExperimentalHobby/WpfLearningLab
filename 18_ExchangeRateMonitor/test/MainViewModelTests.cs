using ExchangeRateMonitor.ViewModels;

namespace ExchangeRateMonitor.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。フェイクの<see cref="FakeExchangeRateApiClient"/>で検証する。
/// リトライ待機は<see cref="TimeSpan.Zero"/>を渡してテストを高速化する。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(FakeExchangeRateApiClient? apiClient = null) =>
		new(apiClient ?? new FakeExchangeRateApiClient(), TimeSpan.Zero);

	/// <summary>
	/// パス条件: AddPairCommand実行で入力した通貨ペアが監視銘柄一覧に追加されること
	/// </summary>
	[Fact]
	public void AddPairCommand_入力した通貨ペアが一覧に追加される()
	{
		var viewModel = CreateViewModel();
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";

		viewModel.AddPairCommand.Execute(null);

		Assert.Single(viewModel.WatchedPairs);
		Assert.Equal("USD/JPY", viewModel.WatchedPairs[0].DisplayName);
	}

	/// <summary>
	/// パス条件: 通貨コードが3文字の英字でない場合、AddPairCommandのCanExecuteがfalseになること
	/// </summary>
	[Theory]
	[InlineData("US", "JPY")]
	[InlineData("USD", "JP1")]
	[InlineData("", "JPY")]
	[InlineData("USD", "")]
	public void AddPairCommand_通貨コードが不正だとCanExecuteがfalseになる(string baseCurrency, string quoteCurrency)
	{
		var viewModel = CreateViewModel();
		viewModel.InputBaseCurrency = baseCurrency;
		viewModel.InputQuoteCurrency = quoteCurrency;

		Assert.False(viewModel.AddPairCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 既に登録済みの通貨ペアはAddPairCommandのCanExecuteがfalseになること(重複防止)
	/// </summary>
	[Fact]
	public void AddPairCommand_登録済みの通貨ペアはCanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		viewModel.InputBaseCurrency = "usd";
		viewModel.InputQuoteCurrency = "jpy";

		Assert.False(viewModel.AddPairCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: RemovePairCommand実行で指定した監視銘柄が一覧から削除されること
	/// </summary>
	[Fact]
	public void RemovePairCommand_指定した銘柄が一覧から削除される()
	{
		var viewModel = CreateViewModel();
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);
		var pair = viewModel.WatchedPairs[0];

		viewModel.RemovePairCommand.Execute(pair);

		Assert.Empty(viewModel.WatchedPairs);
	}

	/// <summary>
	/// パス条件: RefreshAllCommand実行で登録済み銘柄のレートが更新されること
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_登録済み銘柄のレートが更新される()
	{
		var apiClient = new FakeExchangeRateApiClient();
		apiClient.EnqueueSuccess(150.00m);
		var viewModel = CreateViewModel(apiClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		await RunRefreshAllAsync(viewModel);

		Assert.Equal(150.00m, viewModel.WatchedPairs[0].CurrentRate);
	}

	/// <summary>
	/// パス条件: 2回目の一括更新で前回値との比較結果(Trend)が反映されること
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_2回目の更新で前回比較のTrendが反映される()
	{
		var apiClient = new FakeExchangeRateApiClient();
		apiClient.EnqueueSuccess(150.00m);
		apiClient.EnqueueSuccess(151.00m);
		var viewModel = CreateViewModel(apiClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);
		await RunRefreshAllAsync(viewModel);

		await RunRefreshAllAsync(viewModel);

		Assert.Equal(RateTrend.Up, viewModel.WatchedPairs[0].Trend);
	}

	/// <summary>
	/// パス条件: 一部銘柄の取得が(リトライ後も)全て失敗しても、他の銘柄の更新は継続されること
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_一部銘柄が失敗しても他の銘柄は更新される()
	{
		var failingClient = new FakeExchangeRateApiClient();
		failingClient.EnqueueFailure();
		failingClient.EnqueueFailure();
		failingClient.EnqueueFailure();
		failingClient.EnqueueSuccess(160.00m);
		var viewModel = CreateViewModel(failingClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);
		viewModel.InputBaseCurrency = "EUR";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		await RunRefreshAllAsync(viewModel);

		Assert.NotEqual(string.Empty, viewModel.WatchedPairs[0].ErrorMessage);
		Assert.Equal(160.00m, viewModel.WatchedPairs[1].CurrentRate);
	}

	/// <summary>
	/// パス条件: 取得失敗時は指定回数(3回)リトライしてから最終的にエラーとすること
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_失敗時は3回リトライしてからエラーになる()
	{
		var apiClient = new FakeExchangeRateApiClient();
		apiClient.EnqueueFailure();
		apiClient.EnqueueFailure();
		apiClient.EnqueueFailure();
		var viewModel = CreateViewModel(apiClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		await RunRefreshAllAsync(viewModel);

		Assert.Equal(3, apiClient.CallCount);
		Assert.NotEqual(string.Empty, viewModel.WatchedPairs[0].ErrorMessage);
	}

	/// <summary>
	/// パス条件: 異常なAPIレスポンス(レート値が数値として不正、FormatException)でも
	/// 例外を投げずリトライされ、最終的にエラーメッセージが設定されること
	/// (RefreshAllCommandはAsyncRelayCommand経由のasync voidのため、
	/// catch対象外の例外は未処理例外でアプリ全体をクラッシュさせる)
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_FormatException発生時もクラッシュせずエラーになる()
	{
		var apiClient = new FakeExchangeRateApiClient();
		apiClient.EnqueueFormatFailure();
		apiClient.EnqueueFormatFailure();
		apiClient.EnqueueFormatFailure();
		var viewModel = CreateViewModel(apiClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		var exception = await Record.ExceptionAsync(() => RunRefreshAllAsync(viewModel));

		Assert.Null(exception);
		Assert.Equal(3, apiClient.CallCount);
		Assert.NotEqual(string.Empty, viewModel.WatchedPairs[0].ErrorMessage);
	}

	/// <summary>
	/// パス条件: リトライ後に成功すればエラーメッセージが設定されないこと
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_リトライ後に成功すればエラーにならない()
	{
		var apiClient = new FakeExchangeRateApiClient();
		apiClient.EnqueueFailure();
		apiClient.EnqueueSuccess(150.00m);
		var viewModel = CreateViewModel(apiClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		await RunRefreshAllAsync(viewModel);

		Assert.Equal(2, apiClient.CallCount);
		Assert.Equal(string.Empty, viewModel.WatchedPairs[0].ErrorMessage);
		Assert.Equal(150.00m, viewModel.WatchedPairs[0].CurrentRate);
	}

	/// <summary>
	/// パス条件: RefreshAllCommand実行後、LastUpdatedが更新されること
	/// </summary>
	[Fact]
	public async Task RefreshAllCommand_実行後にLastUpdatedが更新される()
	{
		var apiClient = new FakeExchangeRateApiClient();
		apiClient.EnqueueSuccess(150.00m);
		var viewModel = CreateViewModel(apiClient);
		viewModel.InputBaseCurrency = "USD";
		viewModel.InputQuoteCurrency = "JPY";
		viewModel.AddPairCommand.Execute(null);

		Assert.Null(viewModel.LastUpdated);
		await RunRefreshAllAsync(viewModel);

		Assert.NotNull(viewModel.LastUpdated);
	}

	/// <summary>
	/// AsyncRelayCommandはExecute(void)経由で実行されるためawaitできない。
	/// テストではTaskCompletionSourceを使わず、コマンドの完了をポーリングして待つ。
	/// </summary>
	private static async Task RunRefreshAllAsync(MainViewModel viewModel)
	{
		viewModel.RefreshAllCommand.Execute(null);
		while (!viewModel.RefreshAllCommand.CanExecute(null))
		{
			await Task.Delay(10);
		}
	}
}
