namespace StickyNotes;

/// <summary>
/// 付箋1件分のデータ(位置・サイズ・内容・背景色)。
/// </summary>
public class StickyNoteData
{
	/// <summary>
	/// 付箋を一意に識別するID。
	/// </summary>
	public required string Id { get; init; }

	/// <summary>
	/// 付箋の本文。
	/// </summary>
	public string Text { get; set; } = string.Empty;

	/// <summary>
	/// 画面左端からの位置。
	/// </summary>
	public double Left { get; set; }

	/// <summary>
	/// 画面上端からの位置。
	/// </summary>
	public double Top { get; set; }

	/// <summary>
	/// 付箋の幅。
	/// </summary>
	public double Width { get; set; } = 220;

	/// <summary>
	/// 付箋の高さ。
	/// </summary>
	public double Height { get; set; } = 220;

	/// <summary>
	/// 付箋の背景色(#RRGGBB形式)。
	/// </summary>
	public string ColorHex { get; set; } = "#FFF9C4";
}
