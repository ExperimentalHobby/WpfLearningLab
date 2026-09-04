using PluginNoteApp.Contracts;

namespace PluginNoteApp.Services;

/// <summary>
/// 1つのDLLファイルに対するプラグイン読込結果。
/// </summary>
/// <param name="PluginName">読み込めた場合はプラグイン名、失敗した場合は対象DLLのファイル名。</param>
/// <param name="Plugin">読み込めたプラグインのインスタンス。失敗した場合は<see langword="null"/>。</param>
/// <param name="ErrorMessage">読込に失敗した場合のエラーメッセージ。成功した場合は<see langword="null"/>。</param>
public record PluginLoadResult(string PluginName, IMemoPlugin? Plugin, string? ErrorMessage)
{
	/// <summary>読込に成功したかどうか。</summary>
	public bool Success => Plugin is not null;
}
