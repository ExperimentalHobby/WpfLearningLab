using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace ThemeAndLocaleApp;

/// <summary>
/// resxリソースをUI Cultureに応じて取得するためのバインド用シングルトン。
/// XAML側は <c>Binding Path=[キー名]</c> の形でインデクサ経由で参照する。
/// 言語切替時に <see cref="Refresh"/> を呼ぶと、"Item[]" の変更通知により
/// バインドしている全ての文言が再評価される
/// (ObservableCollectionのインデクサ変更通知と同じ規約を利用している)。
/// </summary>
public class LocalizedStrings : INotifyPropertyChanged
{
    private static readonly ResourceManager ResourceManagerInstance =
        new("ThemeAndLocaleApp.Resources.Strings", typeof(LocalizedStrings).Assembly);

    /// <summary>アプリ全体で共有するインスタンス。</summary>
    public static LocalizedStrings Instance { get; } = new();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 指定したキーの文言を、現在の <see cref="CultureInfo.CurrentUICulture"/> で取得する。
    /// </summary>
    public string this[string key] => ResourceManagerInstance.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    /// <summary>
    /// 言語切替後に呼び出し、バインドしている全ての文言を再評価させる。
    /// </summary>
    public void Refresh()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}
