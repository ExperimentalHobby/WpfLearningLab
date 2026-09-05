using System.Windows;
using System.Windows.Media;

namespace ColorPalette;

/// <summary>
/// 色選択パレットのメインウィンドウ。
/// RGB⇔HEX変換は <see cref="ColorPaletteEngine"/> に委譲し、Sliderの操作とHEX入力を相互に反映させるだけの薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly ColorPaletteEngine _engine = new();

	// SliderやHexTextBoxの値をコードから更新している間は、
	// 対応するイベントハンドラでの再帰的な更新を抑止する。
	private bool _isUpdatingFromCode;

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		UpdatePreviewAndHexFromSliders();
	}

	/// <summary>
	/// R/G/B いずれかのSliderの値が変わったときの処理。プレビュー色とHEXコードを更新する。
	/// </summary>
	private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (_isUpdatingFromCode)
		{
			return;
		}

		UpdatePreviewAndHexFromSliders();
	}

	/// <summary>
	/// HEXコード入力欄の内容が変わったときの処理。有効なHEXコードならSliderとプレビューに反映し、
	/// 不正な場合はエラーメッセージを表示するだけで例外は出さない。
	/// </summary>
	private void HexTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		if (_isUpdatingFromCode)
		{
			return;
		}

		if (!_engine.TryParseHex(HexTextBox.Text, out var r, out var g, out var b))
		{
			ErrorTextBlock.Visibility = Visibility.Visible;
			return;
		}

		ErrorTextBlock.Visibility = Visibility.Collapsed;

		_isUpdatingFromCode = true;
		try
		{
			RedSlider.Value = r;
			GreenSlider.Value = g;
			BlueSlider.Value = b;
		}
		finally
		{
			_isUpdatingFromCode = false;
		}

		UpdatePreviewAndLabels(r, g, b);
	}

	/// <summary>
	/// Sliderの現在値からプレビュー・数値ラベル・HEXコード欄をまとめて更新する。
	/// </summary>
	private void UpdatePreviewAndHexFromSliders()
	{
		var r = ToByte(RedSlider.Value);
		var g = ToByte(GreenSlider.Value);
		var b = ToByte(BlueSlider.Value);

		UpdatePreviewAndLabels(r, g, b);

		_isUpdatingFromCode = true;
		try
		{
			HexTextBox.Text = _engine.ToHex(r, g, b);
			ErrorTextBlock.Visibility = Visibility.Collapsed;
		}
		finally
		{
			_isUpdatingFromCode = false;
		}
	}

	/// <summary>
	/// Sliderの値(double)をbyteに変換する。(byte)への直キャストはuncheckedのため
	/// 範囲外の値が意図しないbyte値に化けてしまうので、0〜255にクランプしてから
	/// 四捨五入して変換する。
	/// </summary>
	private static byte ToByte(double sliderValue) =>
		(byte)Math.Round(Math.Clamp(sliderValue, 0, 255), MidpointRounding.AwayFromZero);

	/// <summary>
	/// プレビュー矩形の色と各Slider横の数値ラベルを更新する。
	/// </summary>
	private void UpdatePreviewAndLabels(byte r, byte g, byte b)
	{
		PreviewRectangle.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
		RedValueTextBlock.Text = r.ToString();
		GreenValueTextBlock.Text = g.ToString();
		BlueValueTextBlock.Text = b.ToString();
	}
}
