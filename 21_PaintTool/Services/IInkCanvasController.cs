using System.Windows.Media;

namespace PaintTool.Services;

/// <summary>
/// 実際の描画キャンバス(InkCanvas)に対する操作の抽象。ViewModelをWPF Ink API非依存にする。
/// </summary>
public interface IInkCanvasController
{
	/// <summary>Undoできる操作があるかどうか。</summary>
	bool CanUndo { get; }

	/// <summary>Redoできる操作があるかどうか。</summary>
	bool CanRedo { get; }

	/// <summary>
	/// <see cref="CanUndo"/>/<see cref="CanRedo"/>が変化した(描画・消去・Undo・Redoが行われた)ときに発火する。
	/// </summary>
	event EventHandler? StateChanged;

	/// <summary>直前の操作を取り消す。</summary>
	void Undo();

	/// <summary>取り消した操作をやり直す。</summary>
	void Redo();

	/// <summary>描画内容を全て消去する(この操作もUndo可能)。</summary>
	void ClearAll();

	/// <summary>ペンの色を設定する。</summary>
	void SetPenColor(Color color);

	/// <summary>ペインの太さを設定する。</summary>
	void SetPenWidth(double width);

	/// <summary>消しゴムモードの有効/無効を設定する。</summary>
	void SetEraserMode(bool isEraser);

	/// <summary>描画内容をPNG画像として指定パスに保存する。</summary>
	void SaveAsPng(string filePath);
}
