namespace ThemeAndLocaleApp.Models;

/// <summary>
/// 選択中のテーマ・言語設定。
/// </summary>
public class AppSettings
{
	/// <summary>テーマ名("Light" または "Dark")。</summary>
	public string Theme { get; set; } = "Light";

	/// <summary>言語のカルチャコード("ja" または "en")。</summary>
	public string Culture { get; set; } = "ja";
}
