using FileOrganizer.Services;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際の<see cref="System.IO.FileSystemWatcher"/>を使わない<see cref="IDirectoryWatcher"/>実装。
/// </summary>
public class FakeDirectoryWatcher : IDirectoryWatcher
{
	/// <inheritdoc/>
	public event Action<string>? FileCreated;

	/// <summary>最後に<see cref="Start"/>が呼ばれたフォルダのパス。</summary>
	public string? StartedFolder { get; private set; }

	/// <summary>監視中かどうか。</summary>
	public bool IsStarted { get; private set; }

	/// <summary><see cref="Dispose"/>が呼ばれた回数。</summary>
	public int DisposeCallCount { get; private set; }

	/// <summary><see cref="FileCreated"/>に購読者が残っているかどうか。</summary>
	public bool HasFileCreatedSubscribers => FileCreated is not null;

	/// <inheritdoc/>
	public void Start(string folderPath)
	{
		StartedFolder = folderPath;
		IsStarted = true;
	}

	/// <inheritdoc/>
	public void Stop() => IsStarted = false;

	/// <inheritdoc/>
	public void Dispose()
	{
		DisposeCallCount++;
		Stop();
	}

	/// <summary>
	/// テストからファイル作成イベントを疑似的に発火させる。
	/// </summary>
	public void RaiseFileCreated(string filePath) => FileCreated?.Invoke(filePath);
}
