using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace UnitConverter;

/// <summary>
/// 単位変換ツールのメインウィンドウ。
/// カテゴリ・単位の選択と数値入力を <see cref="UnitConverterEngine"/> に委譲するだけの薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly UnitConverterEngine _engine = new();

	// 単位ComboBoxのItemsSource/SelectedIndexをコードから更新している間は
	// SelectionChangedイベント経由での変換実行(まだ両方の単位が揃っていない状態での実行)を抑止する。
	private bool _isUpdatingUnits;

	/// <summary>
	/// ウィンドウを初期化し、カテゴリ一覧を表示する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		CategoryComboBox.ItemsSource = _engine.Categories;
		CategoryComboBox.SelectedIndex = 0;
	}

	/// <summary>
	/// カテゴリが変更されたときの処理。単位の選択肢を入れ替えてから再変換する。
	/// </summary>
	private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		UpdateUnitChoices();
		UpdateConversion();
	}

	/// <summary>
	/// 変換元/変換先の単位が変更されたときの処理。
	/// </summary>
	private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_isUpdatingUnits)
		{
			return;
		}

		UpdateConversion();
	}

	/// <summary>
	/// 変換元の数値が変更されたときの処理。
	/// </summary>
	private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		UpdateConversion();
	}

	/// <summary>
	/// 選択中のカテゴリに応じて、変換元/変換先の単位ComboBoxの選択肢を入れ替える。
	/// </summary>
	private void UpdateUnitChoices()
	{
		if (CategoryComboBox.SelectedItem is not string category)
		{
			return;
		}

		_isUpdatingUnits = true;

		var units = _engine.GetUnits(category);
		FromUnitComboBox.ItemsSource = units;
		ToUnitComboBox.ItemsSource = units;
		FromUnitComboBox.SelectedIndex = 0;
		ToUnitComboBox.SelectedIndex = units.Count > 1 ? 1 : 0;

		_isUpdatingUnits = false;
	}

	/// <summary>
	/// 現在のカテゴリ・単位・入力値をもとに変換を実行し、結果を表示する。
	/// 数値以外や空の入力の場合は変換先を空表示にし、例外は出さない。
	/// </summary>
	private void UpdateConversion()
	{
		if (CategoryComboBox.SelectedItem is not string category ||
			FromUnitComboBox.SelectedItem is not string fromUnit ||
			ToUnitComboBox.SelectedItem is not string toUnit)
		{
			return;
		}

		if (!decimal.TryParse(InputTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
		{
			OutputTextBox.Text = string.Empty;
			return;
		}

		var result = _engine.Convert(category, value, fromUnit, toUnit);
		OutputTextBox.Text = Math.Round(result, 6, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture);
	}
}
