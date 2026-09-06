using Microsoft.Win32;

namespace SystemTrayUtility.Services;

/// <summary>
/// <c>HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run</c>への登録でWindows起動時の自動起動を実現する
/// <see cref="IStartupRegistrar"/>の実装。
/// </summary>
public class RegistryStartupRegistrar : IStartupRegistrar
{
	private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

	private readonly string _valueName;
	private readonly string _executablePath;

	/// <summary>
	/// <see cref="RegistryStartupRegistrar"/>を初期化する。
	/// </summary>
	/// <param name="valueName">Runキーに登録する値の名前(アプリを識別する一意な名前)。</param>
	/// <param name="executablePath">自動起動時に実行する実行ファイルのパス。</param>
	public RegistryStartupRegistrar(string valueName, string executablePath)
	{
		_valueName = valueName;
		_executablePath = executablePath;
	}

	/// <inheritdoc/>
	public bool IsRegistered()
	{
		using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
		return key?.GetValue(_valueName) is string value && value == _executablePath;
	}

	/// <inheritdoc/>
	public void Register()
	{
		if (string.IsNullOrEmpty(_executablePath))
		{
			throw new InvalidOperationException("実行ファイルのパスを取得できなかったため、スタートアップに登録できません。");
		}

		using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
			?? Registry.CurrentUser.CreateSubKey(RunKeyPath);
		key.SetValue(_valueName, _executablePath);
	}

	/// <inheritdoc/>
	public void Unregister()
	{
		using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
		key?.DeleteValue(_valueName, throwOnMissingValue: false);
	}
}
