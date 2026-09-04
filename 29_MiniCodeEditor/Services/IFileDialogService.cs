namespace MiniCodeEditor.Services;

/// <summary>
/// ファイルを開く/保存するダイアログの抽象。
/// ViewModelのテストで実際にダイアログを開かずに済むように分離する。
/// </summary>
public interface IFileDialogService
{
	/// <summary>
	/// ファイルを開くダイアログを表示する。キャンセルされた場合は<see langword="null"/>を返す。
	/// </summary>
	string? ShowOpenDialog();

	/// <summary>
	/// ファイルを保存するダイアログを表示する。キャンセルされた場合は<see langword="null"/>を返す。
	/// </summary>
	/// <param name="suggestedFileName">初期表示するファイル名。</param>
	string? ShowSaveDialog(string? suggestedFileName);
}
