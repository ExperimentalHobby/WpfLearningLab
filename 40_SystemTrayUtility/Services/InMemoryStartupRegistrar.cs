namespace SystemTrayUtility.Services;

/// <summary>
/// 実レジストリへ触れない<see cref="IStartupRegistrar"/>の実装。
/// UI Automationでの動作確認時に、実機のスタートアップ登録を汚さないよう<see cref="RegistryStartupRegistrar"/>の代わりに使う。
/// </summary>
public class InMemoryStartupRegistrar : IStartupRegistrar
{
	private bool _isRegistered;

	/// <inheritdoc/>
	public bool IsRegistered() => _isRegistered;

	/// <inheritdoc/>
	public void Register() => _isRegistered = true;

	/// <inheritdoc/>
	public void Unregister() => _isRegistered = false;
}
