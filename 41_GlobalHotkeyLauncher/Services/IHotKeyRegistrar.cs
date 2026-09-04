using GlobalHotkeyLauncher.Models;

namespace GlobalHotkeyLauncher.Services;

/// <summary>
/// OSへのグローバルホットキー登録を抽象化する。
/// </summary>
public interface IHotKeyRegistrar
{
	/// <summary>
	/// 指定したIDでホットキーの組み合わせを登録する。
	/// </summary>
	/// <param name="id">登録に使う一意なID。</param>
	/// <param name="combination">登録する組み合わせ。</param>
	/// <returns>登録に成功した場合は<see langword="true"/>(他アプリが既に使用中の場合などはfalse)。</returns>
	bool TryRegister(int id, HotKeyCombination combination);

	/// <summary>
	/// 指定したIDのホットキー登録を解除する。
	/// </summary>
	/// <param name="id">解除するID。</param>
	void Unregister(int id);
}
