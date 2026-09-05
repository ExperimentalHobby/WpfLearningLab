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
        var dictionary = new ResourceDictionary { Source = new Uri($"Themes/{fileName}", UriKind.Relative) };

        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(dictionary);

        _settings.Theme = theme;
    }

    private void ApplyCulture(string culture)
    {
        var cultureInfo = new CultureInfo(culture);
        CultureInfo.CurrentUICulture = cultureInfo;
        _settings.Culture = culture;
    }

    private void SaveSettings()
    {
        _settingsStore.Save(_settings);
    }
}
