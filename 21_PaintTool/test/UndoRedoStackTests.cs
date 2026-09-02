using PaintTool.Services;

namespace PaintTool.Tests;

/// <summary>
/// <see cref="UndoRedoStack{T}"/> の単体テスト。
/// </summary>
public class UndoRedoStackTests
{
	/// <summary>
	/// パス条件: 初期状態ではCanUndo/CanRedoが共にfalseであること
	/// </summary>
	[Fact]
	public void 初期状態はCanUndoとCanRedoが共にfalse()
	{
		var stack = new UndoRedoStack<string>();

		Assert.False(stack.CanUndo);
		Assert.False(stack.CanRedo);
	}

	/// <summary>
	/// パス条件: Pushすると対象項目に対しCanUndoがtrueになること
	/// </summary>
	[Fact]
	public void Push後はCanUndoがtrueになる()
	{
		var stack = new UndoRedoStack<string>();

		stack.Push("線A");

		Assert.True(stack.CanUndo);
		Assert.False(stack.CanRedo);
	}

	/// <summary>
	/// パス条件: UndoでPushした項目が取得でき、1件のみの場合Undo後CanUndoがfalseに戻ること
	/// </summary>
	[Fact]
	public void Undoでpushした項目が取得でき1件のみならCanUndoがfalseに戻る()
	{
		var stack = new UndoRedoStack<string>();
		stack.Push("線A");

		var undone = stack.Undo();

		Assert.Equal("線A", undone);
		Assert.False(stack.CanUndo);
	}

	/// <summary>
	/// パス条件: Undo後CanRedoがtrueになり、Redoで同じ項目が再取得できること
	/// </summary>
	[Fact]
	public void Undo後CanRedoがtrueになりRedoで同じ項目が取得できる()
	{
		var stack = new UndoRedoStack<string>();
		stack.Push("線A");
		stack.Undo();

		Assert.True(stack.CanRedo);
		var redone = stack.Redo();

		Assert.Equal("線A", redone);
		Assert.True(stack.CanUndo);
		Assert.False(stack.CanRedo);
	}

	/// <summary>
	/// パス条件: Undo後に新規Pushすると、Redoスタックがクリアされやり直しできなくなること
	/// </summary>
	[Fact]
	public void Undo後に新規Pushするとやり直しできなくなる()
	{
		var stack = new UndoRedoStack<string>();
		stack.Push("線A");
		stack.Undo();

		stack.Push("線B");

		Assert.False(stack.CanRedo);
	}

	/// <summary>
	/// パス条件: Clearで両スタックが空になること
	/// </summary>
	[Fact]
	public void Clearで両スタックが空になる()
	{
		var stack = new UndoRedoStack<string>();
		stack.Push("線A");
		stack.Undo();

		stack.Clear();

		Assert.False(stack.CanUndo);
		Assert.False(stack.CanRedo);
	}
}
