using GlobalHotkeyLauncher.Models;
using GlobalHotkeyLauncher.Services;

namespace GlobalHotkeyLauncher.Tests;

/// <summary>
/// <see cref="IHotKeyRegistrar"/> のテスト用フェイク。実際のOS登録は行わず、呼び出し内容を記録する。
/// </summary>
public class FakeHotKeyRegistrar : IHotKeyRegistrar
{
	/// <summary>
	/// <see cref="TryRegister"/>に渡されたIDの一覧(呼び出し順)。
	/// </summary>
	public List<int> RegisteredIds { get; } = [];

	/// <summary>
	/// <see cref="Unregister"/>に渡されたIDの一覧(呼び出し順)。
	/// </summary>
	public List<int> UnregisteredIds { get; } = [];

	/// <summary>
	/// 次回の<see cref="TryRegister"/>呼び出しが返す結果。既定はtrue(登録成功)。
	/// </summary>
	public bool NextRegisterResult { get; set; } = true;

	/// <inheritdoc/>
	public bool TryRegister(int id, HotKeyCombination combination)
	{
		if (!NextRegisterResult)
		{
			return false;
		}

		RegisteredIds.Add(id);
		return true;
	}

	/// <inheritdoc/>
	public void Unregister(int id) => UnregisteredIds.Add(id);
}
