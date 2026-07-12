using System.IO;

namespace NotepadClone;

/// <summary>
/// メモ帳クローンのテキスト内容・ファイルパス・未保存状態を管理し、
/// ウィンドウタイトルの生成を行うエンジン。実際のファイルI/O・ダイアログ表示は行わない。
/// </summary>
public class NotepadEngine
{
	/// <summary>
	/// 現在編集中のテキスト内容。
	/// </summary>
	public string Text { get; private set; } = string.Empty;

	/// <summary>
	/// 現在編集中のファイルの保存先パス。未保存(無題)の場合は null。
	/// </summary>
	public string? FilePath { get; private set; }

	/// <summary>
	/// 直近の読込・保存以降に未保存の変更があるかどうか。
	/// </summary>
	public bool IsDirty { get; private set; }

	/// <summary>
	/// 現在の状態からウィンドウタイトルを生成する。
	/// 未保存の変更があれば先頭に "*"、ファイル名が未確定なら "無題" を表示する。
	/// </summary>
	public string GetWindowTitle()
	{
		var name = FilePath is null ? "無題" : Path.GetFileName(FilePath);
		var mark = IsDirty ? "*" : string.Empty;
		return $"{mark}{name} - メモ帳";
	}

	/// <summary>
	/// 編集中のテキストを更新し、未保存状態にする。
	/// </summary>
	/// <param name="text">更新後のテキスト内容。</param>
	public void UpdateText(string text)
	{
		Text = text;
		IsDirty = true;
	}

	/// <summary>
	/// ファイルから読み込んだ内容を反映する。読み込み直後は未保存の変更がない状態になる。
	/// </summary>
	/// <param name="filePath">読み込んだファイルのパス。</param>
	/// <param name="content">読み込んだファイルの内容。</param>
	public void Load(string filePath, string content)
	{
		Text = content;
		FilePath = filePath;
		IsDirty = false;
	}

	/// <summary>
	/// ファイルへの保存が完了したことを反映する。
	/// </summary>
	/// <param name="filePath">保存先のファイルパス。</param>
	public void MarkSaved(string filePath)
	{
		FilePath = filePath;
		IsDirty = false;
	}

	/// <summary>
	/// 編集内容・ファイルパス・未保存状態を初期状態にリセットする。
	/// </summary>
	public void New()
	{
		Text = string.Empty;
		FilePath = null;
		IsDirty = false;
	}
}
