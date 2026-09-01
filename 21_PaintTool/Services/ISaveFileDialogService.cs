namespace PaintTool.Services;

/// <summary>
/// 保存先ファイルパスをユーザーに選択させるダイアログの抽象。
/// </summary>
public interface ISaveFileDialogService
{
	/// <summary>
	/// 保存先パスの選択をユーザーに求める。キャンセルされた場合は<see langword="null"/>を返す。
	/// </summary>
	/// <param name="defaultExtension">既定の拡張子(例: "png")。</param>
	/// <param name="filter">ファイルダイアログのフィルタ文字列。</param>
	string? PromptForSavePath(string defaultExtension, string filter);
}
