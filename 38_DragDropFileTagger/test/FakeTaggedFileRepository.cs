using DragDropFileTagger.Data;
using DragDropFileTagger.Models;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテストで実ファイルI/Oを行わずに済む<see cref="ITaggedFileRepository"/>のフェイク。
/// </summary>
internal class FakeTaggedFileRepository : ITaggedFileRepository
{
	public List<TaggedFile> SavedFiles { get; private set; } = [];

	public int SaveCallCount { get; private set; }

	public List<TaggedFile> Load() => SavedFiles;

	public void Save(IReadOnlyList<TaggedFile> files)
	{
		SavedFiles = files.ToList();
		SaveCallCount++;
	}
}
