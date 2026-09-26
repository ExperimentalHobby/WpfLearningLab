using System.Windows.Media;
using PaintTool.Services;

namespace PaintTool.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IInkCanvasController"/>のフェイク実装。
/// </summary>
public class FakeInkCanvasController : IInkCanvasController
{
	private bool _canUndo;
	private bool _canRedo;

	/// <inheritdoc/>
	public bool CanUndo
	{
		get => _canUndo;
		set
		{
			_canUndo = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	/// <inheritdoc/>
	public bool CanRedo
	{
		get => _canRedo;
		set
		{
			_canRedo = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	/// <inheritdoc/>
	public event EventHandler? StateChanged;

	/// <summary><see cref="Undo"/>が呼ばれた回数(テスト用)。</summary>
	public int UndoCallCount { get; private set; }

	/// <summary><see cref="Redo"/>が呼ばれた回数(テスト用)。</summary>
	public int RedoCallCount { get; private set; }

	/// <summary><see cref="ClearAll"/>が呼ばれた回数(テスト用)。</summary>
	public int ClearAllCallCount { get; private set; }

	/// <summary>直近に<see cref="SetPenColor"/>で設定されたペン色(テスト用)。</summary>
	public Color? LastPenColor { get; private set; }

	/// <summary>直近に<see cref="SetPenWidth"/>で設定されたペン幅(テスト用)。</summary>
	public double? LastPenWidth { get; private set; }

	/// <summary>直近に<see cref="SetEraserMode"/>で設定された値(テスト用)。</summary>
	public bool? LastEraserMode { get; private set; }

	/// <summary>直近に<see cref="SaveAsPng"/>で保存されたパス(テスト用)。</summary>
	public string? LastSavedPath { get; private set; }

	/// <summary>設定すると<see cref="SaveAsPng"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? SaveExceptionToThrow { get; set; }

	/// <inheritdoc/>
	public void Undo() => UndoCallCount++;

	/// <inheritdoc/>
	public void Redo() => RedoCallCount++;

	/// <inheritdoc/>
	public void ClearAll() => ClearAllCallCount++;

	/// <inheritdoc/>
	public void SetPenColor(Color color) => LastPenColor = color;

	/// <inheritdoc/>
	public void SetPenWidth(double width) => LastPenWidth = width;

	/// <inheritdoc/>
	public void SetEraserMode(bool isEraser) => LastEraserMode = isEraser;

	/// <inheritdoc/>
	public void SaveAsPng(string filePath)
	{
		if (SaveExceptionToThrow is not null)
		{
			throw SaveExceptionToThrow;
		}

		LastSavedPath = filePath;
	}
}
