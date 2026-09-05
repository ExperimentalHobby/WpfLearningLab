namespace ScreenCaptureTool.Services;

/// <summary>
/// 保存先ファイルパスの選択UIを担う抽象。
/// </summary>
public interface ISaveFileDialogService
{
	/// <summary>
	/// 保存先ファイルパスの選択ダイアログを表示する。
	/// </summary>
	/// <param name="path">選択されたパス。キャンセル時は<see langword="null"/>。</param>
	/// <returns>パスが選択された場合は<see langword="true"/>。</returns>
	bool TryGetSavePath(out string? path);
}
