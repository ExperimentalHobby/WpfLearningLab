using SystemTrayUtility.Services;

namespace SystemTrayUtility.Tests;

/// <summary>
/// <see cref="RegistryStartupRegistrar"/>のテスト。
/// 実際のHKCUレジストリキーへテスト専用の値名(実行毎に一意)で読み書きし、テスト後に必ず削除する。
/// </summary>
public class RegistryStartupRegistrarTests : IDisposable
{
	private readonly RegistryStartupRegistrar _registrar;

	public RegistryStartupRegistrarTests()
	{
		var valueName = $"SystemTrayUtilityTests_{Guid.NewGuid():N}";
		_registrar = new RegistryStartupRegistrar(valueName, @"C:\dummy\SystemTrayUtility.exe");
	}

	public void Dispose()
	{
		_registrar.Unregister();
	}

	/// <summary>
	/// パス条件: Register前はIsRegisteredがfalseを返すこと。
	/// </summary>
	[Fact]
	public void IsRegistered_Register前はfalse()
	{
		Assert.False(_registrar.IsRegistered());
	}

	/// <summary>
	/// パス条件: Register後はIsRegisteredがtrueを返すこと。
	/// </summary>
	[Fact]
	public void IsRegistered_Register後はtrue()
	{
		_registrar.Register();

		Assert.True(_registrar.IsRegistered());
	}

	/// <summary>
	/// パス条件: Register後にUnregisterすると、IsRegisteredがfalseに戻ること。
	/// </summary>
	[Fact]
	public void IsRegistered_Unregister後はfalseに戻る()
	{
		_registrar.Register();

		_registrar.Unregister();

		Assert.False(_registrar.IsRegistered());
	}

	/// <summary>
	/// パス条件: 実行ファイルパスが空文字の場合、Registerが例外をスローし、空文字のままレジストリに登録されないこと。
	/// </summary>
	[Fact]
	public void Register_実行ファイルパスが空の場合は例外がスローされる()
	{
		var registrar = new RegistryStartupRegistrar($"SystemTrayUtilityTests_{Guid.NewGuid():N}", string.Empty);

		Assert.Throws<InvalidOperationException>(() => registrar.Register());
	}
}
