using System.Windows;
using System.Windows.Controls;

namespace BmiCalculator;

/// <summary>
/// BMI計算機のメインウィンドウ。
/// 入力検証は <see cref="NumericRangeValidationRule"/>、BMI計算・判定は <see cref="BmiEngine"/> に委譲する薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly BmiEngine _engine = new();

	/// <summary>
	/// 身長入力欄(cm)にバインドする文字列。
	/// </summary>
	public string HeightInput { get; set; } = string.Empty;

	/// <summary>
	/// 体重入力欄(kg)にバインドする文字列。
	/// </summary>
	public string WeightInput { get; set; } = string.Empty;

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		DataContext = this;
		UpdateCalculateButtonState();
	}

	/// <summary>
	/// 「計算」ボタン押下時の処理。入力値からBMIを計算し、判定区分とともに表示する。
	/// </summary>
	private void CalculateButton_Click(object sender, RoutedEventArgs e)
	{
		if (!decimal.TryParse(HeightTextBox.Text, out var height) ||
			!decimal.TryParse(WeightTextBox.Text, out var weight))
		{
			return;
		}

		var bmi = _engine.CalculateBmi(height, weight);
		var category = _engine.JudgeCategory(bmi);

		BmiValueTextBlock.Text = $"BMI: {Math.Round(bmi, 2)}";
		JudgmentTextBlock.Text = category;
		JudgmentTextBlock.Tag = category;
	}

	/// <summary>
	/// 入力欄の検証エラー状態が変化したときの処理。計算ボタンの有効/無効を更新する。
	/// </summary>
	private void RootGrid_ValidationError(object sender, ValidationErrorEventArgs e)
	{
		UpdateCalculateButtonState();
	}

	/// <summary>
	/// 身長・体重の両方が検証エラーなしかつ未入力でない場合のみ、計算ボタンを有効にする。
	/// </summary>
	private void UpdateCalculateButtonState()
	{
		var hasError = Validation.GetHasError(HeightTextBox) || Validation.GetHasError(WeightTextBox);
		var hasEmptyInput = string.IsNullOrWhiteSpace(HeightTextBox.Text) || string.IsNullOrWhiteSpace(WeightTextBox.Text);
		CalculateButton.IsEnabled = !hasError && !hasEmptyInput;
	}
}
