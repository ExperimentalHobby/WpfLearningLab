using MiniCodeEditor.Services;

namespace MiniCodeEditor.ViewModels;

/// <summary>
/// コード編集・ファイルの新規/開く/保存を行うメイン画面のViewModel。
/// エディタ本体の表示内容は<see cref="IEditorController"/>経由で操作する。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IEditorController _editor;
	private readonly IFileDialogService _dialogService;
	private readonly IFileService _fileService;

	private string? _currentFilePath;
	private bool _isWordWrapEnabled;
	private bool _showLineNumbers = true;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel(IEditorController editor, IFileDialogService dialogService, IFileService fileService)
	{
		_editor = editor;
		_dialogService = dialogService;
		_fileService = fileService;

		NewCommand = new RelayCommand(New);
		OpenCommand = new RelayCommand(Open);
		SaveCommand = new RelayCommand(Save);
		SaveAsCommand = new RelayCommand(SaveAs);
	}

	/// <summary>現在開いているファイルのパス。未保存の場合は<see langword="null"/>。</summary>
	public string? CurrentFilePath
	{
		get => _currentFilePath;
		private set => SetProperty(ref _currentFilePath, value);
	}

	/// <summary>折り返し表示を有効にするかどうか。</summary>
	public bool IsWordWrapEnabled
	{
		get => _isWordWrapEnabled;
		set => SetProperty(ref _isWordWrapEnabled, value);
	}

	/// <summary>行番号を表示するかどうか。</summary>
	public bool ShowLineNumbers
	{
		get => _showLineNumbers;
		set => SetProperty(ref _showLineNumbers, value);
	}

	/// <summary>新規作成コマンド。</summary>
	public RelayCommand NewCommand { get; }

	/// <summary>ファイルを開くコマンド。</summary>
	public RelayCommand OpenCommand { get; }

	/// <summary>上書き保存コマンド(未保存の場合は名前を付けて保存と同じ動作)。</summary>
	public RelayCommand SaveCommand { get; }

	/// <summary>名前を付けて保存コマンド。</summary>
	public RelayCommand SaveAsCommand { get; }

	private void New()
	{
		_editor.Text = string.Empty;
		CurrentFilePath = null;
		_editor.SetSyntaxHighlighting(null);
	}

	private void Open()
	{
		var filePath = _dialogService.ShowOpenDialog();
		if (filePath is null)
		{
			return;
		}

		_editor.Text = _fileService.ReadAllText(filePath);
		CurrentFilePath = filePath;
		_editor.SetSyntaxHighlighting(filePath);
	}

	private void Save()
	{
		if (CurrentFilePath is null)
		{
			SaveAs();
			return;
		}

		_fileService.WriteAllText(CurrentFilePath, _editor.Text);
	}

	private void SaveAs()
	{
		var filePath = _dialogService.ShowSaveDialog(CurrentFilePath);
		if (filePath is null)
		{
			return;
		}

		_fileService.WriteAllText(filePath, _editor.Text);
		CurrentFilePath = filePath;
		_editor.SetSyntaxHighlighting(filePath);
	}
}
