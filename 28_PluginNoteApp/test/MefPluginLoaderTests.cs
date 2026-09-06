using PluginNoteApp.Services;

namespace PluginNoteApp.Tests;

/// <summary>
/// <see cref="MefPluginLoader"/> の単体テスト。
/// 実際にビルドされたCharacterCountPlugin.dll・実の一時フォルダに対して検証する(モック不使用)。
/// </summary>
public class MefPluginLoaderTests : IDisposable
{
	private readonly string _tempDirectory;

	public MefPluginLoaderTests()
	{
		_tempDirectory = Path.Combine(Path.GetTempPath(), $"PluginNoteAppTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_tempDirectory);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDirectory))
		{
			Directory.Delete(_tempDirectory, recursive: true);
		}
	}

	/// <summary>
	/// テストプロジェクトが参照しているビルド済みプラグインDLLの実際の出力パスを返す。
	/// CharacterCountPlugin.csproj/PluginNoteApp.Contracts.csprojをProjectReferenceしているため、
	/// テスト実行前に必ずビルドされている。
	/// Configuration(Debug/Release)はテストアセンブリ自身の出力パスから取得し、
	/// CI(Release実行)とローカル(Debug実行)の両方で解決できるようにする。
	/// </summary>
	private static string GetBuiltDllPath(string projectName, string fileName)
	{
		var testAssemblyDir = Path.GetDirectoryName(typeof(MefPluginLoaderTests).Assembly.Location)!;
		// bin/PluginNoteApp.Tests/<Configuration>/net10.0-windows/ -> bin/<projectName>/<Configuration>/<tfm>/
		var configurationDir = Directory.GetParent(testAssemblyDir)!;
		var binDir = configurationDir.Parent!.Parent!.FullName;
		return Path.Combine(binDir, projectName, configurationDir.Name, "net10.0", fileName);
	}

	private static string GetCharacterCountPluginDllPath() =>
		GetBuiltDllPath("CharacterCountPlugin", "CharacterCountPlugin.dll");

	/// <summary>
	/// パス条件: 実際にビルドされたプラグインDLLを読み込むと、IMemoPluginとして取得できること
	/// </summary>
	[Fact]
	public void LoadPlugins_実際のプラグインDLLを読み込むとIMemoPluginとして取得できる()
	{
		var pluginDllPath = GetCharacterCountPluginDllPath();
		File.Copy(pluginDllPath, Path.Combine(_tempDirectory, "CharacterCountPlugin.dll"));
		var loader = new MefPluginLoader();

		var results = loader.LoadPlugins(_tempDirectory);

		var result = Assert.Single(results);
		Assert.True(result.Success);
		Assert.Equal("文字数カウント", result.Plugin!.Name);
		Assert.Equal("文字数: 3文字", result.Plugin.Process("abc"));
	}

	/// <summary>
	/// パス条件: 壊れたDLLファイルを読み込んでも例外を投げず、失敗として記録されること
	/// </summary>
	[Fact]
	public void LoadPlugins_壊れたDLLは例外を投げず失敗として記録される()
	{
		var brokenDllPath = Path.Combine(_tempDirectory, "Broken.dll");
		File.WriteAllBytes(brokenDllPath, [0x00, 0x01, 0x02, 0x03]);
		var loader = new MefPluginLoader();

		var results = loader.LoadPlugins(_tempDirectory);

		var result = Assert.Single(results);
		Assert.False(result.Success);
		Assert.NotNull(result.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 存在しないフォルダを指定した場合、空の結果を返すこと
	/// </summary>
	[Fact]
	public void LoadPlugins_存在しないフォルダの場合空の結果を返す()
	{
		var loader = new MefPluginLoader();

		var results = loader.LoadPlugins(Path.Combine(_tempDirectory, "not-exist"));

		Assert.Empty(results);
	}

	/// <summary>
	/// パス条件: IMemoPluginをExportしていない(正常だがプラグインではない)DLLの場合、
	/// 失敗としてではなく0件の結果を返すこと
	/// </summary>
	[Fact]
	public void LoadPlugins_IMemoPluginをExportしないDLLは0件の結果を返す()
	{
		var contractsDllPath = GetBuiltDllPath("PluginNoteApp.Contracts", "PluginNoteApp.Contracts.dll");
		File.Copy(contractsDllPath, Path.Combine(_tempDirectory, "PluginNoteApp.Contracts.dll"));
		var loader = new MefPluginLoader();

		var results = loader.LoadPlugins(_tempDirectory);

		Assert.Empty(results);
	}

	/// <summary>
	/// パス条件: 壊れたDLLと正常なプラグインDLLが混在する場合、正常な方は読み込め壊れた方だけ失敗として記録されること
	/// </summary>
	[Fact]
	public void LoadPlugins_壊れたDLLと正常なDLLが混在する場合正常な方は読み込める()
	{
		var pluginDllPath = GetCharacterCountPluginDllPath();
		File.Copy(pluginDllPath, Path.Combine(_tempDirectory, "CharacterCountPlugin.dll"));
		File.WriteAllBytes(Path.Combine(_tempDirectory, "Broken.dll"), [0x00, 0x01, 0x02, 0x03]);
		var loader = new MefPluginLoader();

		var results = loader.LoadPlugins(_tempDirectory);

		Assert.Equal(2, results.Count);
		Assert.Contains(results, r => r.Success);
		Assert.Contains(results, r => !r.Success);
	}

	/// <summary>
	/// パス条件: プラグイン読込後にDisposeしても例外を投げないこと
	/// (AssemblyLoadContextによる分離・アンロード対応の確認)
	/// </summary>
	[Fact]
	public void Dispose_プラグイン読込後に呼んでも例外を投げない()
	{
		var pluginDllPath = GetCharacterCountPluginDllPath();
		File.Copy(pluginDllPath, Path.Combine(_tempDirectory, "CharacterCountPlugin.dll"));
		var loader = new MefPluginLoader();
		var results = loader.LoadPlugins(_tempDirectory);
		Assert.Single(results);

		var exception = Record.Exception(loader.Dispose);

		Assert.Null(exception);
	}
}
