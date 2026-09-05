using System.IO;
using System.Text.Json;
using ThemeAndLocaleApp.Models;

namespace ThemeAndLocaleApp;

/// <summary>
/// テーマ・言語設定をJSONファイルへ保存/読込するロジック。
/// </summary>
public class SettingsStore
{
    private readonly string _filePath;

    public SettingsStore(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// 設定を読み込む。ファイルが存在しない、またはJSONとして不正な場合は既定値を返す。
    /// </summary>
    public AppSettings Load()
    {
        if (!File.Exists(_filePath))
        {
            return new AppSettings();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch (JsonException)
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// 設定をJSONファイルへ保存する。
    /// </summary>
    public void Save(AppSettings settings)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings);
        File.WriteAllText(_filePath, json);
    }
}
