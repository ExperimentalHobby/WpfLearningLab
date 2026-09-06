using SystemTrayUtility.Services;

namespace SystemTrayUtility.Tests.Fakes;

/// <summary>
/// レジストリ操作の失敗(権限不足等)を再現するための、常に例外をスローする<see cref="IStartupRegistrar"/>フェイク。
/// </summary>
internal class ThrowingStartupRegistrar : IStartupRegistrar
{
	/// <inheritdoc/>
	public bool IsRegistered() => throw new UnauthorizedAccessException("test-forced-failure");

	/// <inheritdoc/>
	public void Register() => throw new UnauthorizedAccessException("test-forced-failure");

	/// <inheritdoc/>
	public void Unregister() => throw new UnauthorizedAccessException("test-forced-failure");
}
