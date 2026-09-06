namespace MiniCodeEditor.Services;

/// <summary>
/// 未保存の変更がある状態で新規作成/ファイルを開こうとしたときの、続行方法の確認を行う処理の抽象。
/// </summary>
public interface IUnsavedChangesPrompt
{
	/// <summary>
	/// ユーザーに確認する。
	/// </summary>
	/// <returns>
	/// <see langword="true"/>=保存してから続行する、<see langword="false"/>=変更を破棄して続行する、
	/// <see langword="null"/>=キャンセル(操作を中止する)。
	/// </returns>
	bool? Confirm();
}
