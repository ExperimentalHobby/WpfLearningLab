using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace NotepadClone;

/// <summary>
/// メモ帳クローンのメインウィンドウ。
/// テキストの状態管理を <see cref="NotepadEngine"/> に委譲し、
/// ファイルI/O・ダイアログ表示・ウィンドウタイトル更新のみを担う薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly NotepadEngine _engine = new();
	private bool _suppressTextChanged;

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		UpdateTitle();
	}

	/// <summary>
	/// 「新規作成」メニュー押下時の処理。未保存の変更があれば確認してから内容をクリアする。
	/// </summary>
	private void New_Click(object sender, RoutedEventArgs e)
	{
		if (!ConfirmProceedDespiteUnsavedChanges())
		{
			return;
		}

		_engine.New();
		SetEditorTextWithoutMarkingDirty(string.Empty);
		UpdateTitle();
	}

	/// <summary>
	/// 「開く」メニュー押下時の処理。未保存の変更があれば確認してからファイルを読み込む。
	/// </summary>
	private void Open_Click(object sender, RoutedEventArgs e)
	{
		if (!ConfirmProceedDespiteUnsavedChanges())
		{
			return;
		}

		var dialog = new OpenFileDialog
		{
			Filter = "テキスト ファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
		};

		if (dialog.ShowDialog() != true)
		{
			return;
		}

		string content;
		try
		{
			content = File.ReadAllText(dialog.FileName);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			MessageBox.Show(
				$"ファイルを開けませんでした。\n{ex.Message}",
				"メモ帳クローン",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
			return;
		}

		_engine.Load(dialog.FileName, content);
		SetEditorTextWithoutMarkingDirty(content);
		UpdateTitle();
	}

	/// <summary>
	/// 「保存」メニュー押下時の処理。
	/// </summary>
	private void Save_Click(object sender, RoutedEventArgs e)
	{
		PerformSave();
	}

	/// <summary>
	/// 「名前を付けて保存」メニュー押下時の処理。
	/// </summary>
	private void SaveAs_Click(object sender, RoutedEventArgs e)
	{
		PerformSaveAs();
	}

	/// <summary>
	/// 本文の入力内容が変わったときの処理。プログラムからの読込・新規作成時は反映しない。
	/// </summary>
	private void EditorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		if (_suppressTextChanged)
		{
			return;
		}

		_engine.UpdateText(EditorTextBox.Text);
		UpdateTitle();
	}

	/// <summary>
	/// ウィンドウを閉じようとしたときの処理。未保存の変更があれば確認し、キャンセルされたら閉じない。
	/// </summary>
	private void MainWindow_Closing(object? sender, CancelEventArgs e)
	{
		if (!ConfirmProceedDespiteUnsavedChanges())
		{
			e.Cancel = true;
		}
	}

	/// <summary>
	/// 未保存の変更がある場合に保存するかどうかを確認する。
	/// </summary>
	/// <returns>
	/// 未保存の変更がない、または保存/破棄を選んで処理を続行してよい場合は true。
	/// ユーザーがキャンセルした場合、または保存に失敗した場合は false。
	/// </returns>
	private bool ConfirmProceedDespiteUnsavedChanges()
	{
		if (!_engine.IsDirty)
		{
			return true;
		}

		var result = MessageBox.Show(
			"変更内容を保存しますか?",
			"メモ帳クローン",
			MessageBoxButton.YesNoCancel,
			MessageBoxImage.Warning);

		return result switch
		{
			MessageBoxResult.Yes => PerformSave(),
			MessageBoxResult.No => true,
			_ => false,
		};
	}

	/// <summary>
	/// 現在のファイルパスに保存する。ファイルパスが未確定の場合は名前を付けて保存に切り替える。
	/// </summary>
	/// <returns>保存できた場合は true、キャンセルされた場合は false。</returns>
	private bool PerformSave()
	{
		if (_engine.FilePath is null)
		{
			return PerformSaveAs();
		}

		if (!TryWriteFile(_engine.FilePath, EditorTextBox.Text))
		{
			return false;
		}

		_engine.MarkSaved(_engine.FilePath);
		UpdateTitle();
		return true;
	}

	/// <summary>
	/// 保存先を選択するダイアログを表示し、選択されたパスに保存する。
	/// </summary>
	/// <returns>保存できた場合は true、ダイアログがキャンセルされた場合は false。</returns>
	private bool PerformSaveAs()
	{
		var dialog = new SaveFileDialog
		{
			Filter = "テキスト ファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
			DefaultExt = ".txt",
		};

		if (dialog.ShowDialog() != true)
		{
			return false;
		}

		if (!TryWriteFile(dialog.FileName, EditorTextBox.Text))
		{
			return false;
		}

		_engine.MarkSaved(dialog.FileName);
		UpdateTitle();
		return true;
	}

	/// <summary>
	/// 指定したパスにテキストを書き込む。権限なし・ロック・ディスク満杯等のI/Oエラーは
	/// 例外を投げずにエラーダイアログを表示し、false を返す。
	/// </summary>
	/// <param name="filePath">書き込み先のファイルパス。</param>
	/// <param name="content">書き込む内容。</param>
	/// <returns>書き込みに成功した場合は true。</returns>
	private static bool TryWriteFile(string filePath, string content)
	{
		try
		{
			File.WriteAllText(filePath, content);
			return true;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			MessageBox.Show(
				$"ファイルを保存できませんでした。\n{ex.Message}",
				"メモ帳クローン",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
			return false;
		}
	}

	/// <summary>
	/// TextChanged による未保存マークを立てずに、本文表示だけを更新する。
	/// </summary>
	private void SetEditorTextWithoutMarkingDirty(string text)
	{
		_suppressTextChanged = true;
		try
		{
			EditorTextBox.Text = text;
		}
		finally
		{
			_suppressTextChanged = false;
		}
	}

	/// <summary>
	/// ウィンドウタイトルを <see cref="NotepadEngine"/> の現在の状態に合わせて更新する。
	/// </summary>
	private void UpdateTitle()
	{
		Title = _engine.GetWindowTitle();
	}
}
