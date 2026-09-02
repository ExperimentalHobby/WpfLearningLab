namespace PluginNoteApp.Tests;

/// <summary>
/// サンプルプラグイン<see cref="global::CharacterCountPlugin.CharacterCountPlugin"/>の単体テスト。
/// </summary>
public class CharacterCountPluginTests
{
	/// <summary>
	/// パス条件: Processを呼ぶと、メモ本文の文字数を含む文字列を返すこと
	/// </summary>
	[Fact]
	public void Process_メモ本文の文字数を含む文字列を返す()
	{
		var plugin = new global::CharacterCountPlugin.CharacterCountPlugin();

		var result = plugin.Process("こんにちは");

		Assert.Equal("文字数: 5文字", result);
	}

	/// <summary>
	/// パス条件: Nameがプラグインの表示名を返すこと
	/// </summary>
	[Fact]
	public void Name_プラグインの表示名を返す()
	{
		var plugin = new global::CharacterCountPlugin.CharacterCountPlugin();

		Assert.Equal("文字数カウント", plugin.Name);
	}
}
