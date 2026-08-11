using System.Net.Http;
using System.Text.Json;
using WeatherApp.Services;

namespace WeatherApp.ViewModels;

/// <summary>
/// 天気予報アプリのメイン画面のViewModel。地名検索・天気情報の表示・エラー処理を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IWeatherApiClient _apiClient;

	private string _searchText = string.Empty;
	private bool _isLoading;
	private string _errorMessage = string.Empty;
	private string _resolvedLocationName = string.Empty;
	private double _temperature;
	private double _humidity;
	private double _windSpeed;
	private string _weatherDescription = string.Empty;
	private string _weatherIcon = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="apiClient">天気情報取得に使うクライアント。</param>
	public MainViewModel(IWeatherApiClient apiClient)
	{
		_apiClient = apiClient;
		SearchCommand = new AsyncRelayCommand(SearchAsync, CanSearch);
	}

	/// <summary>検索する地名の入力欄。</summary>
	public string SearchText
	{
		get => _searchText;
		set
		{
			if (SetProperty(ref _searchText, value))
			{
				SearchCommand.RaiseCanExecuteChanged();
			}
		}
	}

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

	/// <summary>解決された地名。</summary>
	public string ResolvedLocationName
	{
		get => _resolvedLocationName;
		private set => SetProperty(ref _resolvedLocationName, value);
	}

	/// <summary>気温(摂氏)。</summary>
	public double Temperature
	{
		get => _temperature;
		private set => SetProperty(ref _temperature, value);
	}

	/// <summary>相対湿度(%)。</summary>
	public double Humidity
	{
		get => _humidity;
		private set => SetProperty(ref _humidity, value);
	}

	/// <summary>風速(km/h)。</summary>
	public double WindSpeed
	{
		get => _windSpeed;
		private set => SetProperty(ref _windSpeed, value);
	}

	/// <summary>天候の日本語名(例: 曇り)。</summary>
	public string WeatherDescription
	{
		get => _weatherDescription;
		private set => SetProperty(ref _weatherDescription, value);
	}

	/// <summary>天候の絵文字アイコン。</summary>
	public string WeatherIcon
	{
		get => _weatherIcon;
		private set => SetProperty(ref _weatherIcon, value);
	}

	/// <summary>
	/// 地名を検索し、天気情報を取得するコマンド。
	/// </summary>
	public AsyncRelayCommand SearchCommand { get; }

	private bool CanSearch() => !string.IsNullOrWhiteSpace(SearchText);

	private async Task SearchAsync()
	{
		IsLoading = true;
		ErrorMessage = string.Empty;
		try
		{
			var location = await _apiClient.SearchLocationAsync(SearchText);
			if (location is null)
			{
				ErrorMessage = $"「{SearchText}」が見つかりませんでした。";
				return;
			}

			var weather = await _apiClient.GetCurrentWeatherAsync(location.Latitude, location.Longitude);
			ResolvedLocationName = location.Name;
			Temperature = weather.Temperature;
			Humidity = weather.Humidity;
			WindSpeed = weather.WindSpeed;
			WeatherDescription = WeatherCodeMapper.ToDescription(weather.WeatherCode);
			WeatherIcon = WeatherCodeMapper.ToIcon(weather.WeatherCode);
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
		{
			ErrorMessage = "天気情報の取得に失敗しました。通信環境を確認してください。";
		}
		finally
		{
			IsLoading = false;
		}
	}
}
