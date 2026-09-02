namespace PluginNoteApp.Contracts;

/// <summary>
/// メモアプリのプラグインが実装するインターフェース。
/// プラグインDLLはこのインターフェースを実装したクラスに<c>[Export(typeof(IMemoPlugin))]</c>属性を
/// 付けることで、ホストアプリ側からMEF(System.Composition)経由で動的に発見・読込される。
/// </summary>
public interface IMemoPlugin
{
	/// <summary>プラグインの表示名。</summary>
	string Name { get; }

	/// <summary>
	/// メモ本文を処理し、結果の文字列を返す。
	/// </summary>
	/// <param name="memoText">処理対象のメモ本文。</param>
	string Process(string memoText);
}
