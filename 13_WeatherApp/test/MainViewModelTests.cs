using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: SearchCommand実行で地名解決→天気取得の順に呼ばれ、結果が各プロパティに反映されること
	/// </summary>
	[Fact]
	public async Task SearchCommand_実行すると天気情報が取得され各プロパティに反映される()
	{
		var apiClient = new FakeWeatherApiClient
		{
			SearchLocationResult = new GeocodingResult { Name = "東京", Latitude = 35.6895, Longitude = 139.69171 },
			CurrentWeatherResult = new CurrentWeather { Temperature = 30.5, Humidity = 65, WeatherCode = 3, WindSpeed = 12.3 },
		};
		var viewModel = new MainViewModel(apiClient) { SearchText = "東京" };

		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal("東京", viewModel.ResolvedLocationName);
		Assert.Equal(30.5, viewModel.Temperature);
		Assert.Equal(65, viewModel.Humidity);
		Assert.Equal(12.3, viewModel.WindSpeed);
		Assert.Equal("曇り", viewModel.WeatherDescription);
		Assert.Equal("☁️", viewModel.WeatherIcon);
	}

	/// <summary>
	/// パス条件: 検索中はIsLoadingがtrueになり、完了後falseに戻ること
	/// </summary>
	[Fact]
	public async Task SearchCommand_検索中はIsLoadingがtrueになり完了後falseに戻る()
	{
		var gate = new TaskCompletionSource();
		var apiClient = new FakeWeatherApiClient
		{
			SearchLocationGate = gate,
			SearchLocationResult = new GeocodingResult { Name = "東京", Latitude = 35.6895, Longitude = 139.69171 },
			CurrentWeatherResult = new CurrentWeather { Temperature = 30.5, Humidity = 65, WeatherCode = 3, WindSpeed = 12.3 },
		};
		var viewModel = new MainViewModel(apiClient) { SearchText = "東京" };

		viewModel.SearchCommand.Execute(null);
		var isLoadingDuringSearch = viewModel.IsLoading;
		gate.SetResult();
		await Task.Delay(50);

		Assert.True(isLoadingDuringSearch);
		Assert.False(viewModel.IsLoading);
	}

	/// <summary>
	/// パス条件: 地名が見つからない場合、ErrorMessageが設定されること
	/// </summary>
	[Fact]
	public async Task SearchCommand_地名が見つからない場合ErrorMessageが設定される()
	{
		var apiClient = new FakeWeatherApiClient { SearchLocationResult = null };
		var viewModel = new MainViewModel(apiClient) { SearchText = "存在しない地名" };

		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: API呼び出しで例外が発生した場合、ErrorMessageが設定されること(通信失敗時のエラーハンドリング)
	/// </summary>
	[Fact]
	public async Task SearchCommand_通信例外発生時ErrorMessageが設定される()
	{
		var apiClient = new FakeWeatherApiClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = new MainViewModel(apiClient) { SearchText = "東京" };

		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: API仕様変更等でレスポンスに期待するプロパティが無くKeyNotFoundExceptionが
	/// 発生しても、例外を投げずErrorMessageが設定されること
	/// (AsyncRelayCommand.Executeはasync void実装でcatchを持たないため、ここで捕捉し
	/// 損ねると未処理例外でアプリ全体がクラッシュする)。
	/// </summary>
	[Fact]
	public async Task SearchCommand_KeyNotFoundException発生時ErrorMessageが設定される()
	{
		var apiClient = new FakeWeatherApiClient { ExceptionToThrow = new KeyNotFoundException("name") };
		var viewModel = new MainViewModel(apiClient) { SearchText = "東京" };

		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: レスポンスの型が期待と異なりInvalidOperationExceptionが発生しても、
	/// 例外を投げずErrorMessageが設定されること
	/// </summary>
	[Fact]
	public async Task SearchCommand_InvalidOperationException発生時ErrorMessageが設定される()
	{
		var apiClient = new FakeWeatherApiClient { ExceptionToThrow = new InvalidOperationException("型が異なる") };
		var viewModel = new MainViewModel(apiClient) { SearchText = "東京" };

		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 再検索時に前回のErrorMessageがクリアされること
	/// </summary>
	[Fact]
	public async Task SearchCommand_再検索時に前回のErrorMessageがクリアされる()
	{
		var apiClient = new FakeWeatherApiClient { SearchLocationResult = null };
		var viewModel = new MainViewModel(apiClient) { SearchText = "存在しない地名" };
		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);
		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);

		apiClient.SearchLocationResult = new GeocodingResult { Name = "東京", Latitude = 35.6895, Longitude = 139.69171 };
		apiClient.CurrentWeatherResult = new CurrentWeather { Temperature = 30.5, Humidity = 65, WeatherCode = 3, WindSpeed = 12.3 };
		viewModel.SearchText = "東京";

		viewModel.SearchCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: SearchTextが空欄の場合、SearchCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void SearchCommand_SearchTextが空欄の場合CanExecuteがfalseになる(string searchText)
	{
		var apiClient = new FakeWeatherApiClient();
		var viewModel = new MainViewModel(apiClient) { SearchText = searchText };

		var canExecute = viewModel.SearchCommand.CanExecute(null);

		Assert.False(canExecute);
	}

	/// <summary>
	/// パス条件: SearchTextを変更すると、SearchCommandのCanExecuteChangedが発火し、ボタンの有効/無効がUIに追従すること
	/// </summary>
	[Fact]
	public void SearchText_変更するとSearchCommandのCanExecuteChangedが発火する()
	{
		var apiClient = new FakeWeatherApiClient();
		var viewModel = new MainViewModel(apiClient);
		var raised = false;
		viewModel.SearchCommand.CanExecuteChanged += (_, _) => raised = true;

		viewModel.SearchText = "東京";

		Assert.True(raised);
	}
}
