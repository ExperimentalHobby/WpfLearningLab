namespace ToDoList.Tests;

/// <summary>
/// <see cref="ToDoListEngine"/> のタスクCRUD操作に関するテスト。
/// </summary>
public class ToDoListEngineTests
{
	/// <summary>
	/// パス条件: 何もタスクを追加していない初期状態で Tasks が空であること。
	/// </summary>
	[Fact]
	public void InitialTasks_IsEmpty()
	{
		var engine = new ToDoListEngine();

		Assert.Empty(engine.Tasks);
	}

	/// <summary>
	/// パス条件: 有効なタイトルで AddTask すると Tasks に未完了状態のタスクが追加されること。
	/// </summary>
	[Fact]
	public void AddTask_ValidTitle_AddsTaskToTasks()
	{
		var engine = new ToDoListEngine();

		engine.AddTask("牛乳を買う");

		var task = Assert.Single(engine.Tasks);
		Assert.Equal("牛乳を買う", task.Title);
		Assert.False(task.IsDone);
	}

	/// <summary>
	/// パス条件: 前後に空白を含むタイトルで AddTask すると、空白がトリムされて登録されること。
	/// </summary>
	[Fact]
	public void AddTask_TitleWithWhitespace_TrimsTitle()
	{
		var engine = new ToDoListEngine();

		engine.AddTask("  牛乳を買う  ");

		var task = Assert.Single(engine.Tasks);
		Assert.Equal("牛乳を買う", task.Title);
	}

	/// <summary>
	/// パス条件: 空白のみのタイトルで AddTask してもタスクが追加されないこと。
	/// </summary>
	[Fact]
	public void AddTask_BlankTitle_DoesNotAddTask()
	{
		var engine = new ToDoListEngine();

		engine.AddTask("   ");

		Assert.Empty(engine.Tasks);
	}

	/// <summary>
	/// パス条件: AddTask を複数回呼ぶと、各タスクのIdが連番で一意になること。
	/// </summary>
	[Fact]
	public void AddTask_CalledMultipleTimes_AssignsSequentialUniqueIds()
	{
		var engine = new ToDoListEngine();

		engine.AddTask("タスクA");
		engine.AddTask("タスクB");

		Assert.Equal([1, 2], engine.Tasks.Select(t => t.Id));
	}

	/// <summary>
	/// パス条件: 存在するIdを指定して RemoveTask すると、そのタスクが Tasks から削除され true が返ること。
	/// </summary>
	[Fact]
	public void RemoveTask_ExistingId_RemovesTaskAndReturnsTrue()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.RemoveTask(1);

		Assert.True(result);
		Assert.Empty(engine.Tasks);
	}

	/// <summary>
	/// パス条件: 存在しないIdを指定して RemoveTask しても何も削除されず false が返ること。
	/// </summary>
	[Fact]
	public void RemoveTask_NonExistingId_ReturnsFalse()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.RemoveTask(999);

		Assert.False(result);
		Assert.Single(engine.Tasks);
	}

	/// <summary>
	/// パス条件: 未完了のタスクに ToggleDone すると IsDone が true になり、true が返ること。
	/// </summary>
	[Fact]
	public void ToggleDone_UndoneTask_MarksAsDoneAndReturnsTrue()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.ToggleDone(1);

		Assert.True(result);
		Assert.True(engine.Tasks[0].IsDone);
	}

	/// <summary>
	/// パス条件: 完了済みのタスクに ToggleDone すると IsDone が false に戻ること(トグル動作)。
	/// </summary>
	[Fact]
	public void ToggleDone_CalledTwice_RevertsToUndone()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");
		engine.ToggleDone(1);

		engine.ToggleDone(1);

		Assert.False(engine.Tasks[0].IsDone);
	}

	/// <summary>
	/// パス条件: 存在しないIdを指定して ToggleDone すると false が返ること。
	/// </summary>
	[Fact]
	public void ToggleDone_NonExistingId_ReturnsFalse()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.ToggleDone(999);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: 存在するタスクに有効な新タイトルで EditTitle すると、タイトルが変更され true が返ること。
	/// </summary>
	[Fact]
	public void EditTitle_ExistingIdWithValidTitle_UpdatesTitleAndReturnsTrue()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.EditTitle(1, "タスクA改");

		Assert.True(result);
		Assert.Equal("タスクA改", engine.Tasks[0].Title);
	}

	/// <summary>
	/// パス条件: 空白のみの新タイトルで EditTitle してもタイトルが変更されず false が返ること。
	/// </summary>
	[Fact]
	public void EditTitle_BlankTitle_DoesNotUpdateAndReturnsFalse()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.EditTitle(1, "   ");

		Assert.False(result);
		Assert.Equal("タスクA", engine.Tasks[0].Title);
	}

	/// <summary>
	/// パス条件: 存在しないIdを指定して EditTitle すると false が返ること。
	/// </summary>
	[Fact]
	public void EditTitle_NonExistingId_ReturnsFalse()
	{
		var engine = new ToDoListEngine();
		engine.AddTask("タスクA");

		var result = engine.EditTitle(999, "新タイトル");

		Assert.False(result);
	}
}
