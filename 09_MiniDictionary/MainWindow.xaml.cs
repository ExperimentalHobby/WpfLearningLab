using System.Windows;
using System.Windows.Controls;

namespace MiniDictionary;

/// <summary>
/// 簡易電子辞書のメインウィンドウ。
/// 検索・意味取得は <see cref="DictionaryEngine"/> に委譲し、入力に応じた一覧更新と選択時の表示のみを担当する薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private const string NotFoundMessage = "該当する単語がありません";

	private readonly DictionaryEngine _engine = new();

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		UpdateWordList(string.Empty);
	}

	/// <summary>
	/// 検索文字列が変更されたときの処理。候補一覧を絞り込む。
	/// </summary>
	private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		UpdateWordList(SearchTextBox.Text);
	}

	/// <summary>
	/// 候補一覧で単語が選択されたときの処理。意味を表示する。
	/// </summary>
	private void WordListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (WordListBox.SelectedItem is not string word)
		{
			return;
		}

		MeaningTextBlock.Text = _engine.GetMeaning(word) ?? NotFoundMessage;
	}

	/// <summary>
	/// 検索文字列に応じて候補一覧を更新する。該当なしの場合はメッセージを表示する。
	/// </summary>
	private void UpdateWordList(string query)
	{
		var results = _engine.Search(query);
		WordListBox.ItemsSource = results;
		MeaningTextBlock.Text = results.Count == 0 ? NotFoundMessage : string.Empty;
	}
}
