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

	public bool CanUndo
	{
		get => _canUndo;
		set
		{
			_canUndo = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool CanRedo
	{
		get => _canRedo;
		set
		{
			_canRedo = value;
			StateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public event EventHandler? StateChanged;

	public int UndoCallCount { get; private set; }
	public int RedoCallCount { get; private set; }
	public int ClearAllCallCount { get; private set; }
	public Color? LastPenColor { get; private set; }
	public double? LastPenWidth { get; private set; }
	public bool? LastEraserMode { get; private set; }
	public string? LastSavedPath { get; private set; }

	public void Undo() => UndoCallCount++;

	public void Redo() => RedoCallCount++;

	public void ClearAll() => ClearAllCallCount++;

	public void SetPenColor(Color color) => LastPenColor = color;

	public void SetPenWidth(double width) => LastPenWidth = width;

	public void SetEraserMode(bool isEraser) => LastEraserMode = isEraser;

	public void SaveAsPng(string filePath) => LastSavedPath = filePath;
}
