namespace PasswordManager.Services;

/// <summary>
/// クリップボードへのテキストコピーを担う抽象。
/// </summary>
public interface IClipboardService
{
	/// <summary>
	/// 指定した文字列をクリップボードにコピーする。
	/// </summary>
	/// <param name="text">コピーする文字列。</param>
	void SetText(string text);
}
