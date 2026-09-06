using System.Collections.ObjectModel;
using System.IO;

namespace FileTreeExplorer.Models;

/// <summary>
/// TreeViewにバインドするフォルダノード。
/// 未展開時はダミーの子ノードを1つ持ち、展開時に実際のサブフォルダへ置き換える
/// (ダミーノードパターンによる遅延読み込み)。
/// </summary>
public class FolderNode
{
	/// <summary>フォルダ名(表示用)。</summary>
	public string Name { get; }

	/// <summary>フォルダのフルパス。</summary>
	public string FullPath { get; }

	/// <summary>子ノード一覧。未展開時はダミーノード1件のみを含む。</summary>
	public ObservableCollection<FolderNode> Children { get; } = new();

	/// <summary>ダミーノード(プレースホルダー)かどうか。</summary>
	public bool IsPlaceholder { get; }

	/// <summary>実際のサブフォルダを読み込み済みかどうか。</summary>
	public bool IsLoaded { get; private set; }

	public FolderNode(string name, string fullPath, bool addPlaceholder = true, bool isPlaceholder = false)
	{
		Name = name;
		FullPath = fullPath;
		IsPlaceholder = isPlaceholder;

		if (addPlaceholder)
		{
			Children.Add(new FolderNode("読み込み中...", string.Empty, addPlaceholder: false, isPlaceholder: true));
		}
	}

	/// <summary>
	/// 実際のサブフォルダを読み込み、ダミーノードを置き換える。
	/// 読み込みに失敗した場合、子ノードは0件になる。
	/// </summary>
	public void LoadChildren(FileSystemBrowserEngine engine, out string? errorMessage)
	{
		Children.Clear();

		var success = engine.TryGetSubFolders(FullPath, out var folders, out errorMessage);
		if (success)
		{
			foreach (var folder in folders.OrderBy(f => f))
			{
				Children.Add(new FolderNode(Path.GetFileName(folder), folder));
			}
			IsLoaded = true;
		}
		else
		{
			// 失敗時はIsLoadedをtrueにしない(再展開時に再試行できるようにする)。
			// プレースホルダーを復元しないとTreeViewItemの展開矢印が消え、
			// UI上そもそも再展開する手段が無くなってしまう。
			Children.Add(new FolderNode("読み込み中...", string.Empty, addPlaceholder: false, isPlaceholder: true));
		}
	}

	/// <summary>
	/// UI Automationの既定名(型名)ではなく表示名がTreeViewItemの名前として使われるようにする。
	/// </summary>
	public override string ToString() => Name;
}
