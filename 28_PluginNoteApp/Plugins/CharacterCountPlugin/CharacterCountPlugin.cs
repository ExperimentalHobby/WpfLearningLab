using System.Composition;
using PluginNoteApp.Contracts;

namespace CharacterCountPlugin;

/// <summary>
/// メモ本文の文字数をカウントするサンプルプラグイン。
/// </summary>
[Export(typeof(IMemoPlugin))]
public class CharacterCountPlugin : IMemoPlugin
{
	/// <inheritdoc/>
	public string Name => "文字数カウント";

	/// <inheritdoc/>
	public string Process(string memoText) => $"文字数: {memoText.Length}文字";
}
