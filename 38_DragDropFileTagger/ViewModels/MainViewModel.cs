using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using DragDropFileTagger.Data;
using DragDropFileTagger.Models;
using DragDropFileTagger.Services;

namespace DragDropFileTagger.ViewModels;

/// <summary>
/// ドラッグ&amp;ドロップファイルタグ付けツールのメインViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly ITaggedFileRepository _repository;
	private string _filterTag = string.Empty;
	private TaggedFile? _selectedFile;
	private string _newTagInput = string.Empty;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化し、保存済みのファイル一覧を読み込む。
	/// </summary>
	public MainViewModel(ITaggedFileRepository repository)
	{
		_repository = repository;

		foreach (var file in _repository.Load().OrderBy(f => f.SortOrder))
		{
			Files.Add(file);
		}
		RefreshDisplayedFiles();

		AddTagCommand = new RelayCommand(AddTag, CanAddTag);
		RemoveFileCommand = new RelayCommand(RemoveFile, () => SelectedFile is not null);
	}

	/// <summary>取り込んだ全ファイル(並び順)。</summary>
	public ObservableCollection<TaggedFile> Files { get; } = [];

	/// <summary>フィルタ適用後に表示するファイル一覧。</summary>
	public ObservableCollection<TaggedFile> DisplayedFiles { get; } = [];

	/// <summary>絞り込みタグ。空の場合は全件表示。</summary>
	public string FilterTag
	{
		get => _filterTag;
		set
		{
			if (SetProperty(ref _filterTag, value))
			{
				RefreshDisplayedFiles();
			}
		}
	}

	/// <summary>選択中のファイル。</summary>
	public TaggedFile? SelectedFile
	{
		get => _selectedFile;
		set
		{
			if (SetProperty(ref _selectedFile, value))
			{
				((RelayCommand)AddTagCommand).RaiseCanExecuteChanged();
				((RelayCommand)RemoveFileCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>タグ追加フォームの入力値(カンマ区切りで複数指定可)。</summary>
	public string NewTagInput
	{
		get => _newTagInput;
		set
		{
			if (SetProperty(ref _newTagInput, value))
			{
				((RelayCommand)AddTagCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>選択中のファイルにタグを追加するコマンド。</summary>
	public ICommand AddTagCommand { get; }

	/// <summary>選択中のファイルを一覧から削除するコマンド。</summary>
	public ICommand RemoveFileCommand { get; }

	/// <summary>
	/// エクスプローラーからドロップされたファイルパスを取り込む。既に取り込み済みのパスはスキップする。
	/// </summary>
	public void AddFiles(IEnumerable<string> paths)
	{
		var added = false;
		foreach (var path in paths)
		{
			if (!File.Exists(path) || Files.Any(file => file.FilePath == path))
			{
				continue;
			}

			var info = new FileInfo(path);
			Files.Add(new TaggedFile
			{
				FilePath = path,
				SizeBytes = info.Length,
				LastModified = info.LastWriteTime,
				SortOrder = Files.Count,
			});
			added = true;
		}

		if (added)
		{
			RefreshDisplayedFiles();
			Save();
		}
	}

	/// <summary>
	/// アプリ内でのドラッグ操作により、<paramref name="source"/>を<paramref name="target"/>の位置へ移動する。
	/// </summary>
	public void MoveFile(TaggedFile source, TaggedFile target)
	{
		var filesList = Files.ToList();
		var oldIndex = filesList.IndexOf(source);
		var newIndex = filesList.IndexOf(target);
		if (oldIndex < 0 || newIndex < 0)
		{
			return;
		}

		TaggedFileReorderer.Move(filesList, oldIndex, newIndex);

		Files.Clear();
		foreach (var file in filesList)
		{
			Files.Add(file);
		}
		RefreshDisplayedFiles();
		Save();
	}

	private bool CanAddTag() => SelectedFile is not null && !string.IsNullOrWhiteSpace(NewTagInput);

	private void AddTag()
	{
		if (SelectedFile is null)
		{
			return;
		}

		var newTags = NewTagInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (var tag in newTags)
		{
			if (!SelectedFile.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
			{
				SelectedFile.Tags.Add(tag);
			}
		}
		SelectedFile.NotifyTagsChanged();

		NewTagInput = string.Empty;
		RefreshDisplayedFiles();
		Save();
	}

	private void RemoveFile()
	{
		if (SelectedFile is null)
		{
			return;
		}

		Files.Remove(SelectedFile);
		SelectedFile = null;
		RefreshDisplayedFiles();
		Save();
	}

	private void RefreshDisplayedFiles()
	{
		DisplayedFiles.Clear();
		foreach (var file in TaggedFileFilter.Filter(Files.ToList(), FilterTag))
		{
			DisplayedFiles.Add(file);
		}
	}

	private void Save() => _repository.Save(Files.ToList());
}
