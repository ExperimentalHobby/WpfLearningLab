using System.Globalization;
using System.IO;
using System.Windows;
using ThemeAndLocaleApp.Models;

namespace ThemeAndLocaleApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private static readonly string SettingsFilePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"ThemeAndLocaleApp",
		"settings.json");

	private readonly SettingsStore _settingsStore = new(SettingsFilePath);
	private AppSettings _settings = new();

	public MainWindow()
	{
		_settings = _settingsStore.Load();
		ApplyCulture(_settings.Culture);

		InitializeComponent();

		ApplyTheme(_settings.Theme);
	}

	private void LightThemeButton_Click(object sender, RoutedEventArgs e)
	{
		ApplyTheme("Light");
		SaveSettings();
	}

	private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
	{
		ApplyTheme("Dark");
		SaveSettings();
	}

	private void JapaneseButton_Click(object sender, RoutedEventArgs e)
	{
		ApplyCulture("ja");
		LocalizedStrings.Instance.Refresh();
		SaveSettings();
	}

	private void EnglishButton_Click(object sender, RoutedEventArgs e)
	{
		ApplyCulture("en");
		LocalizedStrings.Instance.Refresh();
		SaveSettings();
	}

	private void ApplyTheme(string theme)
	{
		var fileName = theme == "Dark" ? "DarkTheme.xaml" : "LightTheme.xaml";
		var newDictionary = new ResourceDictionary { Source = new Uri($"Themes/{fileName}", UriKind.Relative) };

		// Clear()はApplication.Resources.MergedDictionaries全体を消してしまい、将来テーマ以外の
		// 辞書(コンバータ・共通スタイル等)が追加された場合に巻き添えで消える。テーマ辞書だけを
		// 判別して差し替える。
		var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		var existingThemeDictionaries = mergedDictionaries
			.Where(d => d.Source is { OriginalString: var source } && source.StartsWith("Themes/", StringComparison.Ordinal))
			.ToList();
		foreach (var existing in existingThemeDictionaries)
		{
			mergedDictionaries.Remove(existing);
		}
		mergedDictionaries.Add(newDictionary);

		_settings.Theme = theme;
	}

	private void ApplyCulture(string culture)
	{
		var cultureInfo = CultureResolver.Resolve(culture);
		CultureInfo.CurrentUICulture = cultureInfo;
		// CurrentUICultureはこのスレッドにのみ反映される。切替後に生成される新しいスレッドにも
		// 反映されるよう、既定のスレッドカルチャも設定する。
		CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		_settings.Culture = culture;
	}

	private void SaveSettings()
	{
		_settingsStore.Save(_settings);
	}
}
