using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StickyNotes;

/// <summary>
/// 付箋1枚を表すカスタムウィンドウ。
/// タイトルバーのドラッグによる移動・色スウォッチによる背景色変更・閉じるボタンによる削除を行う。
/// 内容の永続化は上位(<see cref="MainWindow"/>)が <see cref="GetCurrentData"/> を呼び出して行う。
/// </summary>
public partial class StickyNoteWindow : Window
{
	private string _currentColorHex;

	/// <summary>
	/// この付箋のID。
	/// </summary>
	public string NoteId { get; }

	/// <summary>
	/// 指定した付箋データから初期状態を復元してウィンドウを初期化する。
	/// </summary>
	/// <param name="data">初期表示する付箋データ。</param>
	public StickyNoteWindow(StickyNoteData data)
	{
		InitializeComponent();

		NoteId = data.Id;
		Left = data.Left;
		Top = data.Top;
		Width = data.Width;
		Height = data.Height;
		ContentTextBox.Text = data.Text;

		_currentColorHex = data.ColorHex;
		ApplyColor(_currentColorHex);
	}

	/// <summary>
	/// タイトルバーをドラッグしたときの処理。ウィンドウを移動する。
	/// </summary>
	private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		DragMove();
	}

	/// <summary>
	/// 閉じるボタン押下時の処理。この付箋を閉じる(削除)。
	/// </summary>
	private void CloseButton_Click(object sender, RoutedEventArgs e)
	{
		Close();
	}

	/// <summary>
	/// 色スウォッチボタン押下時の処理。付箋の背景色を変更する。
	/// </summary>
	private void ColorButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not System.Windows.Controls.Button { Tag: string colorHex })
		{
			return;
		}

		_currentColorHex = colorHex;
		ApplyColor(colorHex);
	}

	/// <summary>
	/// 付箋の背景色を表す既定色(黄色)。保存データのColorHexが不正な場合のフォールバック用。
	/// </summary>
	private const string DefaultColorHex = "#FFF9C4";

	/// <summary>
	/// 付箋の背景色をHEXコードから適用する。保存ファイルの破損等でHEXコードとして
	/// 不正な文字列が渡された場合、FormatExceptionを投げて起動そのものを失敗させる
	/// のではなく、既定色にフォールバックする。
	/// </summary>
	private void ApplyColor(string colorHex)
	{
		try
		{
			RootBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex));
		}
		catch (FormatException)
		{
			_currentColorHex = DefaultColorHex;
			RootBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DefaultColorHex));
		}
	}

	/// <summary>
	/// この付箋の現在の状態(位置・サイズ・内容・背景色)をデータとして取得する。
	/// </summary>
	public StickyNoteData GetCurrentData() => new()
	{
		Id = NoteId,
		Text = ContentTextBox.Text,
		Left = Left,
		Top = Top,
		Width = Width,
		Height = Height,
		ColorHex = _currentColorHex,
	};
}
