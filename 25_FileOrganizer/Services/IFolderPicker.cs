namespace FileOrganizer.Services;

/// <summary>
/// フォルダ選択ダイアログの抽象。
/// ViewModelのテストで実際にダイアログを開かずに済むように分離する。
/// </summary>
public interface IFolderPicker
{
	/// <summary>
	/// フォルダ選択ダイアログを表示する。キャンセルされた場合は<see langword="null"/>を返す。
	/// </summary>
	string? PickFolder();
}
