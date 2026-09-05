using System.ComponentModel;
using System.IO;
using System.Windows;

namespace StickyNotes;

/// <summary>
/// 付箋アプリのランチャーウィンドウ。
/// 起動時に保存済みの付箋を復元し、終了時に現在開いている付箋を保存する。
/// シリアライズ自体は <see cref="StickyNoteSerializer"/> に委譲し、ここではファイルI/Oと
/// <see cref="StickyNoteWindow"/> の生成・管理のみを行う。
/// </summary>
public partial class MainWindow : Window
{
	private static readonly string SaveFilePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"WpfLearningLab.StickyNotes",
		"notes.json");

	private readonly List<StickyNoteWindow> _noteWindows = [];
	private readonly StickyNoteSerializer _serializer = new();

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		Loaded += MainWindow_Loaded;
		Closing += MainWindow_Closing;
	}

	/// <summary>
	/// 「新規付箋」ボタン押下時の処理。新しい付箋を作成して表示する。
	/// </summary>
	private void NewNoteButton_Click(object sender, RoutedEventArgs e)
	{
		var data = new StickyNoteData
		{
			Id = Guid.NewGuid().ToString(),
			Left = 100 + (_noteWindows.Count * 24),
			Top = 100 + (_noteWindows.Count * 24),
		};
		OpenNoteWindow(data);
	}

	/// <summary>
	/// ウィンドウ読み込み時の処理。保存済みの付箋を復元する。
	/// </summary>
	private void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		if (!File.Exists(SaveFilePath))
		{
			return;
		}

		string json;
		try
		{
			json = File.ReadAllText(SaveFilePath);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			MessageBox.Show(
				$"保存済みの付箋を読み込めませんでした。\n{ex.Message}",
				"付箋",
				MessageBoxButton.OK,
				MessageBoxImage.Warning);
			return;
		}

		foreach (var note in _serializer.Deserialize(json))
		{
			OpenNoteWindow(note);
		}
	}

	/// <summary>
	/// ウィンドウを閉じる(アプリ終了)ときの処理。現在開いている付箋の状態を保存する。
	/// </summary>
	private void MainWindow_Closing(object? sender, CancelEventArgs e)
	{
		SaveNotes();
	}

	/// <summary>
	/// 付箋データから <see cref="StickyNoteWindow"/> を生成し、表示・追跡する。
	/// 個別の付箋が閉じられた時点でも、アプリ終了を待たずに保存する。
	/// </summary>
	private void OpenNoteWindow(StickyNoteData data)
	{
		var window = new StickyNoteWindow(data);
		window.Closed += (_, _) =>
		{
			_noteWindows.Remove(window);
			SaveNotes();
		};
		_noteWindows.Add(window);
		window.Show();
	}

	/// <summary>
	/// 現在開いている付箋の状態をまとめてファイルに保存する。
	/// 書き込みに失敗しても例外を投げず、エラーダイアログを表示するにとどめる。
	/// </summary>
	private void SaveNotes()
	{
		var notes = _noteWindows.Select(w => w.GetCurrentData()).ToList();
		var json = _serializer.Serialize(notes);

		try
		{
			var directory = Path.GetDirectoryName(SaveFilePath);
			if (directory is not null)
			{
				Directory.CreateDirectory(directory);
			}

			File.WriteAllText(SaveFilePath, json);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			MessageBox.Show(
				$"付箋を保存できませんでした。\n{ex.Message}",
				"付箋",
				MessageBoxButton.OK,
				MessageBoxImage.Warning);
		}
	}
}
