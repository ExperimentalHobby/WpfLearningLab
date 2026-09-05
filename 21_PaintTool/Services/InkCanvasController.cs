using System.IO;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PaintTool.Services;

/// <summary>
/// 描画中に発生した1回分の変化(追加/削除されたストローク)。Undo/Redoの単位。
/// </summary>
/// <param name="Added">追加されたストローク。</param>
/// <param name="Removed">削除されたストローク。</param>
internal sealed record StrokeChangeAction(StrokeCollection Added, StrokeCollection Removed);

/// <summary>
/// 実際の<see cref="InkCanvas"/>をラップし、Undo/Redo・ペン設定・PNG保存を行う<see cref="IInkCanvasController"/>実装。
/// </summary>
public class InkCanvasController : IInkCanvasController, IDisposable
{
	private readonly InkCanvas _inkCanvas;
	private readonly UndoRedoStack<StrokeChangeAction> _history = new();
	private bool _isApplyingHistoryChange;

	/// <summary>
	/// コントローラーを初期化し、対象<see cref="InkCanvas"/>のストローク変更を監視する。
	/// </summary>
	/// <param name="inkCanvas">操作対象のInkCanvas。</param>
	public InkCanvasController(InkCanvas inkCanvas)
	{
		_inkCanvas = inkCanvas;
		_inkCanvas.Strokes.StrokesChanged += OnStrokesChanged;
	}

	/// <inheritdoc/>
	public bool CanUndo => _history.CanUndo;

	/// <inheritdoc/>
	public bool CanRedo => _history.CanRedo;

	/// <inheritdoc/>
	public event EventHandler? StateChanged;

	private void OnStrokesChanged(object? sender, StrokeCollectionChangedEventArgs e)
	{
		if (_isApplyingHistoryChange)
		{
			return;
		}

		_history.Push(new StrokeChangeAction(new StrokeCollection(e.Added), new StrokeCollection(e.Removed)));
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <inheritdoc/>
	public void Undo()
	{
		if (!CanUndo)
		{
			return;
		}

		var action = _history.Undo();
		ApplyWithoutHistory(() =>
		{
			_inkCanvas.Strokes.Remove(action.Added);
			_inkCanvas.Strokes.Add(action.Removed);
		});
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <inheritdoc/>
	public void Redo()
	{
		if (!CanRedo)
		{
			return;
		}

		var action = _history.Redo();
		ApplyWithoutHistory(() =>
		{
			_inkCanvas.Strokes.Remove(action.Removed);
			_inkCanvas.Strokes.Add(action.Added);
		});
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <inheritdoc/>
	public void ClearAll() => _inkCanvas.Strokes.Clear();

	/// <inheritdoc/>
	public void SetPenColor(Color color) => _inkCanvas.DefaultDrawingAttributes.Color = color;

	/// <inheritdoc/>
	public void SetPenWidth(double width)
	{
		_inkCanvas.DefaultDrawingAttributes.Width = width;
		_inkCanvas.DefaultDrawingAttributes.Height = width;
	}

	/// <inheritdoc/>
	public void SetEraserMode(bool isEraser) =>
		_inkCanvas.EditingMode = isEraser ? InkCanvasEditingMode.EraseByStroke : InkCanvasEditingMode.Ink;

	/// <inheritdoc/>
	public void SaveAsPng(string filePath)
	{
		if (_inkCanvas.ActualWidth <= 0 || _inkCanvas.ActualHeight <= 0)
		{
			// ウィンドウが最小化中などでActualWidth/Heightが0になっていると、意図せず1px四方の
			// 画像がサイレントに保存されてしまう。呼び出し元(ViewModel)で分かりやすいエラーに
			// できるよう、ここで明示的に例外を送出する。
			throw new InvalidOperationException("キャンバスのサイズが0です。ウィンドウが最小化されている場合は元に戻してから保存してください。");
		}

		var width = (int)_inkCanvas.ActualWidth;
		var height = (int)_inkCanvas.ActualHeight;
		var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
		renderTarget.Render(_inkCanvas);

		var encoder = new PngBitmapEncoder();
		encoder.Frames.Add(BitmapFrame.Create(renderTarget));
		using var stream = File.Create(filePath);
		encoder.Save(stream);
	}

	/// <summary>
	/// <see cref="InkCanvas.Strokes"/>の<see cref="StrokeCollection.StrokesChanged"/>購読を解除する。
	/// </summary>
	public void Dispose()
	{
		_inkCanvas.Strokes.StrokesChanged -= OnStrokesChanged;
		GC.SuppressFinalize(this);
	}

	private void ApplyWithoutHistory(Action action)
	{
		_isApplyingHistoryChange = true;
		try
		{
			action();
		}
		finally
		{
			_isApplyingHistoryChange = false;
		}
	}
}
