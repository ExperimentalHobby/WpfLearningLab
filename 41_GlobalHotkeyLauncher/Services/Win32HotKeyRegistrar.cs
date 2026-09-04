using System.Runtime.InteropServices;
using System.Windows.Input;
using GlobalHotkeyLauncher.Models;

namespace GlobalHotkeyLauncher.Services;

/// <summary>
/// Win32 APIの<c>RegisterHotKey</c>/<c>UnregisterHotKey</c>を使い、実際にOSへグローバルホットキーを
/// 登録する実装。登録先ウィンドウの<c>HWND</c>が確定してから(<c>MainWindow.OnSourceInitialized</c>)
/// 生成する必要がある。
/// </summary>
public sealed class Win32HotKeyRegistrar : IHotKeyRegistrar
{
	/// <summary>
	/// ホットキーを押しっぱなしにしても<c>WM_HOTKEY</c>を連続発火させないためのフラグ。
	/// </summary>
	private const uint ModNoRepeat = 0x4000;

	private readonly IntPtr _hwnd;

	/// <summary>
	/// 登録先ウィンドウのハンドルを指定して初期化する。
	/// </summary>
	/// <param name="hwnd">登録先ウィンドウのハンドル。</param>
	public Win32HotKeyRegistrar(IntPtr hwnd)
	{
		_hwnd = hwnd;
	}

	/// <inheritdoc/>
	public bool TryRegister(int id, HotKeyCombination combination)
	{
		var modifiers = (uint)combination.Modifiers | ModNoRepeat;
		var virtualKey = (uint)KeyInterop.VirtualKeyFromKey(combination.Key);
		return RegisterHotKey(_hwnd, id, modifiers, virtualKey);
	}

	/// <inheritdoc/>
	public void Unregister(int id) => UnregisterHotKey(_hwnd, id);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
