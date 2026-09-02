namespace PluginNoteApp.Services;

/// <summary>
/// 指定フォルダ内のプラグインDLLを動的に読み込む処理の抽象。
/// </summary>
public interface IPluginLoader
{
	/// <summary>
	/// 指定フォルダ内の<c>*.dll</c>を走査し、<see cref="Contracts.IMemoPlugin"/>を実装するプラグインを読み込む。
	/// フォルダが存在しない場合は空の一覧を返す。個々のDLLの読込に失敗しても例外を投げず、
	/// 該当DLL分の結果に失敗として記録し、他のDLLの読込は継続する。
	/// </summary>
	/// <param name="pluginDirectory">プラグインDLLが置かれているフォルダのパス。</param>
	IReadOnlyList<PluginLoadResult> LoadPlugins(string pluginDirectory);
}
