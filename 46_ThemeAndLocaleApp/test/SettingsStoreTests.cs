namespace ThemeAndLocaleApp.Tests;

/// <summary>
/// <see cref="SettingsStore"/> のテスト。
/// </summary>
public class SettingsStoreTests : IDisposable
{
    private readonly string _filePath = Path.Combine(Path.GetTempPath(), $"ThemeAndLocaleApp.Tests.{Guid.NewGuid()}.json");

    /// <summary>
    /// パス条件: 設定ファイルが存在しない状態でLoadすると、既定値のAppSettingsを返すこと
    /// </summary>
    [Fact]
    public void Load_ファイルが存在しない場合は既定値を返す()
    {
        var store = new SettingsStore(_filePath);

        var settings = store.Load();

        Assert.Equal("Light", settings.Theme);
        Assert.Equal("ja", settings.Culture);
    }

    /// <summary>
    /// パス条件: AppSettingsをSaveした後に同じパスからLoadすると、保存した内容が復元されること
    /// </summary>
    [Fact]
    public void Save_保存した内容をLoadで正しく復元できる()
    {
        var store = new SettingsStore(_filePath);
        var settings = new ThemeAndLocaleApp.Models.AppSettings { Theme = "Dark", Culture = "en" };

        store.Save(settings);
        var loaded = store.Load();

        Assert.Equal("Dark", loaded.Theme);
        Assert.Equal("en", loaded.Culture);
    }

    /// <summary>
    /// パス条件: 設定ファイルの中身が不正なJSONの場合、例外を出さず既定値を返すこと
    /// </summary>
    [Fact]
    public void Load_不正なJSONの場合は例外を出さず既定値を返す()
    {
        File.WriteAllText(_filePath, "{ this is not valid json");
        var store = new SettingsStore(_filePath);

        var settings = store.Load();

        Assert.Equal("Light", settings.Theme);
        Assert.Equal("ja", settings.Culture);
    }

    /// <summary>
    /// パス条件: 保存先ディレクトリがまだ存在しない場合でも、Saveがディレクトリを作成し
    /// 例外を出さずに保存できること
    /// </summary>
    [Fact]
    public void Save_保存先ディレクトリが存在しない場合はディレクトリを作成して保存する()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"ThemeAndLocaleApp.Tests.Dir.{Guid.NewGuid()}");
        var filePath = Path.Combine(directory, "settings.json");
        var store = new SettingsStore(filePath);

        try
        {
            store.Save(new ThemeAndLocaleApp.Models.AppSettings { Theme = "Dark", Culture = "en" });
            var loaded = store.Load();

            Assert.Equal("Dark", loaded.Theme);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    public void Dispose()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
