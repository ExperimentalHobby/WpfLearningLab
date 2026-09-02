namespace MiniCodeEditor.Services;

/// <summary>
/// コードエディタ本体(AvalonEditの<see cref="ICSharpCode.AvalonEdit.TextEditor"/>)を操作する処理の抽象。
/// </summary>
public interface IEditorController
{
	/// <summary>エディタの表示内容。</summary>
	string Text { get; set; }

	/// <summary>
	/// ファイルパスの拡張子に応じたシンタックスハイライトを設定する。
	/// 対応する定義が無い場合はハイライト無し(プレーンテキスト)にする。
	/// </summary>
	/// <param name="filePath">ハイライト判定の元になるファイルパス。<see langword="null"/>の場合はハイライト無しにする。</param>
	void SetSyntaxHighlighting(string? filePath);
}
