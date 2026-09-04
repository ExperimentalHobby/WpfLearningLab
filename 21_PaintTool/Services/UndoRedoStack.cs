namespace PaintTool.Services;

/// <summary>
/// 汎用的なUndo/Redoスタック。特定の操作の型に依存しない純粋なスタック管理ロジック。
/// </summary>
/// <typeparam name="T">記録する操作の型。</typeparam>
public class UndoRedoStack<T>
{
	private readonly Stack<T> _undoStack = new();
	private readonly Stack<T> _redoStack = new();

	/// <summary>Undoできる操作があるかどうか。</summary>
	public bool CanUndo => _undoStack.Count > 0;

	/// <summary>Redoできる操作があるかどうか。</summary>
	public bool CanRedo => _redoStack.Count > 0;

	/// <summary>
	/// 新しい操作を記録する。Redoスタックはクリアされる(やり直し履歴は新規操作で無効になる)。
	/// </summary>
	/// <param name="action">記録する操作。</param>
	public void Push(T action)
	{
		_undoStack.Push(action);
		_redoStack.Clear();
	}

	/// <summary>
	/// 直前の操作を取り出し、Redoスタックへ移動する。
	/// </summary>
	/// <returns>取り消す操作。</returns>
	public T Undo()
	{
		var action = _undoStack.Pop();
		_redoStack.Push(action);
		return action;
	}

	/// <summary>
	/// Undoで取り消した操作を取り出し、Undoスタックへ戻す。
	/// </summary>
	/// <returns>やり直す操作。</returns>
	public T Redo()
	{
		var action = _redoStack.Pop();
		_undoStack.Push(action);
		return action;
	}

	/// <summary>
	/// Undo/Redo双方の履歴をクリアする。
	/// </summary>
	public void Clear()
	{
		_undoStack.Clear();
		_redoStack.Clear();
	}
}
