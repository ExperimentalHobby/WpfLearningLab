using System.Windows;
using System.Windows.Interop;
using GlobalHotkeyLauncher.Services;
using GlobalHotkeyLauncher.ViewModels;

namespace GlobalHotkeyLauncher;

/// <summary>
/// グローバルホットキーランチャーのメイン画面。
/// ウィンドウの<c>HWND</c>が確定してから(<see cref="OnSourceInitialized"/>)
/// <see cref="Win32HotKeyRegistrar"/>を生成し、<c>WM_HOTKEY</c>メッセージをフックする。
/// </summary>
public partial class MainWindow : Window
{
	/// <summary>ホットキー発火時にOSから送られてくるウィンドウメッセージ。</summary>
	private const int WmHotKey = 0x0312;

	private MainViewModel? _viewModel;
	private Win32HotKeyRegistrar? _registrar;
	private HwndSource? _hwndSource;

	public MainWindow()
	{
		InitializeComponent();
	}

	/// <inheritdoc/>
	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);

		var hwnd = new WindowInteropHelper(this).Handle;
		_registrar = new Win32HotKeyRegistrar(hwnd);
		_viewModel = new MainViewModel(_registrar, new ProcessCommandLauncher());
		DataContext = _viewModel;

		_hwndSource = HwndSource.FromHwnd(hwnd);
		_hwndSource?.AddHook(WndProc);
	}

	/// <summary>
	/// <c>WM_HOTKEY</c>を受信したら、ウィンドウがアクティブかどうかに関わらず対応する処理を実行する。
	/// </summary>
	private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == WmHotKey && _viewModel is not null)
		{
			_viewModel.HandleHotKeyTriggered(wParam.ToInt32());
			handled = true;
		}

		return IntPtr.Zero;
	}

	/// <inheritdoc/>
	protected override void OnClosed(EventArgs e)
	{
		if (_viewModel is not null && _registrar is not null)
		{
			foreach (var binding in _viewModel.Bindings)
			{
				_registrar.Unregister(binding.Id);
			}
		}
		_hwndSource?.RemoveHook(WndProc);

		base.OnClosed(e);
	}
}
