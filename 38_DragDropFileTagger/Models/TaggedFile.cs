using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace DragDropFileTagger.Models;

/// <summary>
/// 取り込んだファイル1件と、それに付与したタグ・並び順。
/// </summary>
public class TaggedFile : INotifyPropertyChanged
{
	private ObservableCollection<string> _tags = [];

	/// <summary>
	/// <see cref="TaggedFile"/>を初期化する。
	/// </summary>
	public TaggedFile()
	{
		SubscribeTagsChanged();
	}

	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>ファイルの絶対パス。</summary>
	public string FilePath { get; set; } = string.Empty;

	/// <summary>ファイル名(拡張子込み)。</summary>
	public string FileName => Path.GetFileName(FilePath);

	/// <summary>ファイルサイズ(バイト)。</summary>
	public long SizeBytes { get; set; }

	/// <summary>最終更新日時。</summary>
	public DateTime LastModified { get; set; }

	/// <summary>
	/// 付与されたタグの一覧。<see cref="ObservableCollection{T}"/>のため、呼び出し側は
	/// <c>Tags.Add(...)</c>するだけで<see cref="TagsDisplay"/>の変更通知が自動的に発火する
	/// (以前は呼び出し側が明示的な通知メソッドを呼ぶ必要があり、呼び忘れるとUIが更新されない
	/// 問題があった)。
	/// </summary>
	public ObservableCollection<string> Tags
	{
		get => _tags;
		set
		{
			_tags.CollectionChanged -= OnTagsCollectionChanged;
			_tags = value;
			SubscribeTagsChanged();
			OnPropertyChanged(nameof(TagsDisplay));
		}
	}

	/// <summary>表示順。小さい順に並べる。</summary>
	public int SortOrder { get; set; }

	/// <summary>タグをカンマ区切りで結合した表示用文字列。</summary>
	public string TagsDisplay => string.Join(", ", Tags);

	private void SubscribeTagsChanged() => _tags.CollectionChanged += OnTagsCollectionChanged;

	private void OnTagsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
		OnPropertyChanged(nameof(TagsDisplay));

	private void OnPropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
