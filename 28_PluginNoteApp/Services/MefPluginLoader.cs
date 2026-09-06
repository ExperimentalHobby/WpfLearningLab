using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using PluginNoteApp.Contracts;

namespace PluginNoteApp.Services;

/// <summary>
/// System.Composition(軽量MEF)を使い、指定フォルダ内のDLLから<see cref="IMemoPlugin"/>を動的に読み込む実装。
/// プラグインDLLごとに専用の<see cref="AssemblyLoadContext"/>(collectible)へ読み込むことで、
/// 既定のロードコンテキストを汚さずに分離し、将来的なアンロードを可能にする。
/// </summary>
/// <remarks>
/// 学習用リポジトリのサンプルプラグインを想定した対応であり、Authenticode等の署名検証は
/// スコープ外とする(信頼ポリシーの設計など別途の検討が必要な範囲が大きいため)。
/// </remarks>
public class MefPluginLoader : IPluginLoader, IDisposable
{
	// プラグインインスタンスはMainViewModelがアプリ終了まで保持し続けるため、
	// LoadPluginsFromDllの呼び出しごとにコンテナ/コンテキストを破棄してはならない
	// (以前はusing var containerで即座に破棄しており、取得済みプラグインインスタンスが
	// 依存するコンテナが破棄された状態で使われるバグがあった)。ローダー自体がDisposeされる
	// までフィールドに保持し、生存期間を一致させる。
	private readonly List<(AssemblyLoadContext Context, CompositionHost Container)> _loaded = [];

	/// <inheritdoc/>
	public IReadOnlyList<PluginLoadResult> LoadPlugins(string pluginDirectory)
	{
		var results = new List<PluginLoadResult>();
		if (!Directory.Exists(pluginDirectory))
		{
			return results;
		}

		foreach (var dllPath in Directory.EnumerateFiles(pluginDirectory, "*.dll"))
		{
			results.AddRange(LoadPluginsFromDll(dllPath));
		}

		return results;
	}

	private IEnumerable<PluginLoadResult> LoadPluginsFromDll(string dllPath)
	{
		var fileName = Path.GetFileName(dllPath);
		var context = new AssemblyLoadContext($"Plugin_{fileName}", isCollectible: true);
		try
		{
			// LoadFromAssemblyPath はファイルをメモリマップしたまま保持するため、プラグインDLLが
			// 実行中ずっと削除・上書きできなくなってしまう。バイト列として読み込んでから
			// LoadFromStream で読み込むことで、読み込み後はファイルへのハンドルを保持しないようにする。
			using var stream = new MemoryStream(File.ReadAllBytes(dllPath));
			var assembly = context.LoadFromStream(stream);
			var configuration = new ContainerConfiguration().WithAssembly(assembly);
			var container = configuration.CreateContainer();
			var plugins = container.GetExports<IMemoPlugin>().ToList();
			_loaded.Add((context, container));
			return plugins.Select(plugin => new PluginLoadResult(plugin.Name, plugin, null)).ToList();
		}
		catch (Exception ex) when (ex is BadImageFormatException or FileLoadException or FileNotFoundException
			or ReflectionTypeLoadException or CompositionFailedException or TypeLoadException)
		{
			context.Unload();
			return [new PluginLoadResult(fileName, null, ex.Message)];
		}
		catch (Exception ex)
		{
			// プラグインDLLは任意の外部コードであり、上記以外にも何が起きるか予測できないため
			// 最終防衛ラインとして広く捕捉し、他のDLLの読込に影響しないようにする。
			context.Unload();
			return [new PluginLoadResult(fileName, null, ex.Message)];
		}
	}

	/// <summary>
	/// 読み込んだプラグインのコンテナと<see cref="AssemblyLoadContext"/>を破棄・アンロードする。
	/// </summary>
	public void Dispose()
	{
		foreach (var (context, container) in _loaded)
		{
			container.Dispose();
			context.Unload();
		}

		_loaded.Clear();
		GC.SuppressFinalize(this);
	}
}
