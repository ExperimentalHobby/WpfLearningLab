using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginNoteApp.Contracts;

namespace PluginNoteApp.Services;

/// <summary>
/// System.Composition(軽量MEF)を使い、指定フォルダ内のDLLから<see cref="IMemoPlugin"/>を動的に読み込む実装。
/// </summary>
public class MefPluginLoader : IPluginLoader
{
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

	private static IEnumerable<PluginLoadResult> LoadPluginsFromDll(string dllPath)
	{
		var fileName = Path.GetFileName(dllPath);
		try
		{
			var assembly = Assembly.LoadFrom(dllPath);
			var configuration = new ContainerConfiguration().WithAssembly(assembly);
			using var container = configuration.CreateContainer();
			var plugins = container.GetExports<IMemoPlugin>();
			return plugins.Select(plugin => new PluginLoadResult(plugin.Name, plugin, null)).ToList();
		}
		catch (Exception ex) when (ex is BadImageFormatException or FileLoadException or ReflectionTypeLoadException
			or CompositionFailedException or TypeLoadException)
		{
			return [new PluginLoadResult(fileName, null, ex.Message)];
		}
	}
}
