using System.IO;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace MiniCodeEditor.Services;

/// <summary>
/// 実の<see cref="TextEditor"/>(AvalonEdit)を使う<see cref="IEditorController"/>実装。
/// </summary>
public class AvalonEditController(TextEditor textEditor) : IEditorController
{
	/// <inheritdoc/>
	public string Text
	{
		get => textEditor.Text;
		set => textEditor.Text = value;
	}

	/// <inheritdoc/>
	public void SetSyntaxHighlighting(string? filePath)
	{
		var extension = filePath is null ? null : Path.GetExtension(filePath);
		textEditor.SyntaxHighlighting = string.IsNullOrEmpty(extension)
			? null
			: HighlightingManager.Instance.GetDefinitionByExtension(extension);
	}
}
