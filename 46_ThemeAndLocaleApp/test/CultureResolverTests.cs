using ThemeAndLocaleApp;

namespace ThemeAndLocaleApp.Tests;

/// <summary>
/// <see cref="CultureResolver"/> のテスト。
/// </summary>
public class CultureResolverTests
{
	/// <summary>
	/// パス条件: 有効なカルチャコードを指定すると、対応するCultureInfoを返すこと
	/// </summary>
	[Fact]
	public void Resolve_有効なカルチャコードの場合はそのまま解決される()
	{
		var cultureInfo = CultureResolver.Resolve("en");

		Assert.Equal("en", cultureInfo.TwoLetterISOLanguageName);
	}

	/// <summary>
	/// パス条件: 無効なカルチャコードを指定した場合、例外にならずフォールバック(既定"ja")の
	/// CultureInfoを返すこと(設定ファイルが壊れている場合の起動時クラッシュの回帰テスト)。
	/// </summary>
	[Fact]
	public void Resolve_無効なカルチャコードの場合は例外にならずフォールバックする()
	{
		var exception = Record.Exception(() => CultureResolver.Resolve("this-is-not-a-valid-culture"));

		Assert.Null(exception);
		var cultureInfo = CultureResolver.Resolve("this-is-not-a-valid-culture");
		Assert.Equal("ja", cultureInfo.TwoLetterISOLanguageName);
	}
}
