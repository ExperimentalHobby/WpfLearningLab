using LocalTaskScheduler.Models;
using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="JsonFileTaskRepository"/> の単体テスト。実の一時ファイルに対して検証する。
/// </summary>
public class JsonFileTaskRepositoryTests : IDisposable
{
	private readonly string _filePath;

	public JsonFileTaskRepositoryTests()
	{
		_filePath = Path.Combine(Path.GetTempPath(), $"LocalTaskSchedulerTests_{Guid.NewGuid():N}.json");
	}

	public void Dispose()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}

	/// <summary>
	/// パス条件: Saveしたタスク一覧を、Loadで(ID・各フィールドとも)復元できること
	/// </summary>
	[Fact]
	public void Save_保存したタスク一覧をLoadで復元できる()
	{
		var repository = new JsonFileTaskRepository(_filePath);
		var executeAt = new DateTime(2026, 8, 31, 15, 0, 0);
		var onceTask = new ScheduledTask("バックアップ", ScheduleType.Once, executeAt, null)
		{
			IsEnabled = false,
			LastExecutedAt = executeAt,
		};
		var intervalTask = new ScheduledTask("定期チェック", ScheduleType.Interval, null, TimeSpan.FromMinutes(30));

		repository.Save([onceTask, intervalTask]);
		var loaded = repository.Load();

		Assert.Equal(2, loaded.Count);
		var loadedOnce = loaded.Single(t => t.Name == "バックアップ");
		Assert.Equal(onceTask.Id, loadedOnce.Id);
		Assert.Equal(ScheduleType.Once, loadedOnce.ScheduleType);
		Assert.Equal(executeAt, loadedOnce.ExecuteAt);
		Assert.False(loadedOnce.IsEnabled);
		Assert.Equal(executeAt, loadedOnce.LastExecutedAt);

		var loadedInterval = loaded.Single(t => t.Name == "定期チェック");
		Assert.Equal(intervalTask.Id, loadedInterval.Id);
		Assert.Equal(TimeSpan.FromMinutes(30), loadedInterval.Interval);
		Assert.True(loadedInterval.IsEnabled);
	}

	/// <summary>
	/// パス条件: ファイルが存在しない場合、Loadは空の一覧を返すこと
	/// </summary>
	[Fact]
	public void Load_ファイルが存在しない場合空の一覧を返す()
	{
		var repository = new JsonFileTaskRepository(_filePath);

		Assert.Empty(repository.Load());
	}

	/// <summary>
	/// パス条件: 壊れたJSONファイルを読み込んでも例外を投げず、空の一覧を返すこと
	/// </summary>
	[Fact]
	public void Load_壊れたJSONファイルは例外を投げず空の一覧を返す()
	{
		File.WriteAllText(_filePath, "{ this is not valid json");
		var repository = new JsonFileTaskRepository(_filePath);

		var result = repository.Load();

		Assert.Empty(result);
	}

	/// <summary>
	/// パス条件: Saveを2回実行すると、前回の内容が完全に置き換わること
	/// </summary>
	[Fact]
	public void Save_2回実行すると前回の内容が置き換わる()
	{
		var repository = new JsonFileTaskRepository(_filePath);
		repository.Save([new ScheduledTask("旧タスク", ScheduleType.Once, DateTime.Now, null)]);

		repository.Save([new ScheduledTask("新タスク", ScheduleType.Interval, null, TimeSpan.FromMinutes(5))]);
		var loaded = repository.Load();

		var task = Assert.Single(loaded);
		Assert.Equal("新タスク", task.Name);
	}
}
