using System.ComponentModel;
using System.IO;

namespace DragDropFileTagger.Models;

/// <summary>
/// 取り込んだファイル1件と、それに付与したタグ・並び順。
/// </summary>
public class TaggedFile : INotifyPropertyChanged
{
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

	/// <summary>付与されたタグの一覧。</summary>
	public List<string> Tags { get; set; } = [];

	/// <summary>表示順。小さい順に並べる。</summary>
	public int SortOrder { get; set; }

	/// <summary>タグをカンマ区切りで結合した表示用文字列。</summary>
	public string TagsDisplay => string.Join(", ", Tags);

	/// <summary>
	/// <see cref="Tags"/>を直接変更した後に呼び出し、<see cref="TagsDisplay"/>の変更をUIへ通知する。
	/// </summary>
	public void NotifyTagsChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagsDisplay)));
}
