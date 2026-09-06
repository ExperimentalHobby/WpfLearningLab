using System.IO;
using System.Text.Json;
using LocalTaskScheduler.Models;

namespace LocalTaskScheduler.Services;

/// <summary>
/// 指定したJSONファイルにタスク一覧を保存する<see cref="ITaskRepository"/>実装。
/// </summary>
public class JsonFileTaskRepository : ITaskRepository
{
	private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

	private readonly string _filePath;

	/// <summary>
	/// リポジトリを初期化する。
	/// </summary>
	/// <param name="filePath">タスク一覧を保存するJSONファイルのパス。</param>
	public JsonFileTaskRepository(string filePath)
	{
		_filePath = filePath;
	}

	/// <inheritdoc/>
	public IReadOnlyList<ScheduledTask> Load()
	{
		if (!File.Exists(_filePath))
		{
			return [];
		}

		try
		{
			var json = File.ReadAllText(_filePath);
			var dtos = JsonSerializer.Deserialize<List<ScheduledTaskDto>>(json) ?? [];
			return dtos.Select(ToTask).ToList();
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
		{
			// 保存ファイルが壊れている・読み込めない場合でも、アプリの起動自体はできるようにする
			// (タスクが空の状態から再開できる)。
			return [];
		}
	}

	/// <inheritdoc/>
	public void Save(IReadOnlyList<ScheduledTask> tasks)
	{
		try
		{
			var directory = Path.GetDirectoryName(_filePath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var dtos = tasks.Select(ToDto).ToList();
			var json = JsonSerializer.Serialize(dtos, SerializerOptions);
			File.WriteAllText(_filePath, json);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			// 保存に失敗しても(ディスク容量不足・権限不足等)、タスク管理自体は続行できるようにする。
			System.Diagnostics.Debug.WriteLine($"タスクの保存に失敗しました: {ex.Message}");
		}
	}

	private static ScheduledTask ToTask(ScheduledTaskDto dto) =>
		new(dto.Id, dto.Name, dto.ScheduleType, dto.ExecuteAt, dto.Interval)
		{
			LastExecutedAt = dto.LastExecutedAt,
			IsEnabled = dto.IsEnabled,
		};

	private static ScheduledTaskDto ToDto(ScheduledTask task) => new()
	{
		Id = task.Id,
		Name = task.Name,
		ScheduleType = task.ScheduleType,
		ExecuteAt = task.ExecuteAt,
		Interval = task.Interval,
		LastExecutedAt = task.LastExecutedAt,
		IsEnabled = task.IsEnabled,
	};
}
