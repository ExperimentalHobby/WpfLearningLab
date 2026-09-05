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

	/// <summary>
	/// クリップボードの現在の内容が<paramref name="expectedText"/>と一致する場合のみクリアする。
	/// コピー後にユーザーが別の内容をクリップボードにコピーしていた場合、それを上書きしないための確認。
	/// </summary>
	/// <param name="expectedText">クリアしてよいことを確認するための、コピー時の文字列。</param>
	void ClearIfUnchanged(string expectedText);
}
