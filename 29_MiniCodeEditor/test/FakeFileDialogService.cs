using MiniCodeEditor.Services;

namespace MiniCodeEditor.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のダイアログを開かない<see cref="IFileDialogService"/>実装。
/// </summary>
public class FakeFileDialogService : IFileDialogService
{
	/// <summary><see cref="ShowOpenDialog"/>が返す値(未設定時は<see langword="null"/>、キャンセル相当)。</summary>
	public string? OpenDialogResult { get; set; }

	/// <summary><see cref="ShowSaveDialog"/>が返す値(未設定時は<see langword="null"/>、キャンセル相当)。</summary>
	public string? SaveDialogResult { get; set; }

	/// <summary><see cref="ShowSaveDialog"/>に渡された初期ファイル名。</summary>
	public string? LastSuggestedFileName { get; private set; }

	/// <inheritdoc/>
	public string? ShowOpenDialog() => OpenDialogResult;

	/// <inheritdoc/>
	public string? ShowSaveDialog(string? suggestedFileName)
	{
		LastSuggestedFileName = suggestedFileName;
		return SaveDialogResult;
	}
}
