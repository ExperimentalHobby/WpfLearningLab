using System.IO;

namespace FileOrganizer.Services;

/// <summary>
/// 実の<see cref="System.IO.FileSystemWatcher"/>を使う<see cref="IDirectoryWatcher"/>実装。
/// </summary>
public class FileSystemDirectoryWatcher : IDirectoryWatcher
{
	private FileSystemWatcher? _watcher;

	/// <inheritdoc/>
	public event Action<string>? FileCreated;

	/// <inheritdoc/>
	public void Start(string folderPath)
	{
		Stop();

		_watcher = new FileSystemWatcher(folderPath)
		{
			IncludeSubdirectories = false,
			NotifyFilter = NotifyFilters.FileName,
		};
		_watcher.Created += OnCreated;
		_watcher.EnableRaisingEvents = true;
	}

	/// <inheritdoc/>
	public void Stop()
	{
		if (_watcher is null)
		{
			return;
		}

		_watcher.EnableRaisingEvents = false;
		_watcher.Created -= OnCreated;
		_watcher.Dispose();
		_watcher = null;
	}

	/// <inheritdoc/>
	public void Dispose() => Stop();

	private void OnCreated(object sender, FileSystemEventArgs e) => FileCreated?.Invoke(e.FullPath);
}
