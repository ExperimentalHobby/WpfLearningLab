using System.IO;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace MiniCodeEditor.Services;

/// <summary>
/// 実の<see cref="TextEditor"/>(AvalonEdit)を使う<see cref="IEditorController"/>実装。
/// </summary>
public class AvalonEditController : IEditorController
{
	private readonly TextEditor _textEditor;

	/// <summary>
	/// コントローラーを初期化する。
	/// </summary>
	/// <param name="textEditor">操作対象のTextEditor。</param>
	public AvalonEditController(TextEditor textEditor)
	{
		_textEditor = textEditor;
		_textEditor.TextChanged += (_, _) => TextChanged?.Invoke(this, EventArgs.Empty);
	}

	/// <inheritdoc/>
	public string Text
	{
		get => _textEditor.Text;
		set => _textEditor.Text = value;
	}

	/// <inheritdoc/>
	public event EventHandler? TextChanged;

	/// <inheritdoc/>
	public void SetSyntaxHighlighting(string? filePath)
	{
		var extension = filePath is null ? null : Path.GetExtension(filePath);
		_textEditor.SyntaxHighlighting = string.IsNullOrEmpty(extension)
			? null
			: HighlightingManager.Instance.GetDefinitionByExtension(extension);
	}
}
