using System.IO;
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
	private readonly IUnsavedChangesPrompt _unsavedChangesPrompt;

	private string? _currentFilePath;
	private bool _isWordWrapEnabled;
	private bool _showLineNumbers = true;
	private bool _isDirty;
	private string _errorMessage = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel(
		IEditorController editor,
		IFileDialogService dialogService,
		IFileService fileService,
		IUnsavedChangesPrompt unsavedChangesPrompt)
	{
		_editor = editor;
		_dialogService = dialogService;
		_fileService = fileService;
		_unsavedChangesPrompt = unsavedChangesPrompt;
		_editor.TextChanged += (_, _) => IsDirty = true;

		NewCommand = new RelayCommand(New);
		OpenCommand = new RelayCommand(Open);
		SaveCommand = new RelayCommand(() => Save());
		SaveAsCommand = new RelayCommand(() => SaveAs());
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

	/// <summary>直近の読込・保存以降に未保存の変更があるかどうか。</summary>
	public bool IsDirty
	{
		get => _isDirty;
		private set => SetProperty(ref _isDirty, value);
	}

	/// <summary>直近の操作で発生したエラーメッセージ。エラーがなければ空文字列。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
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
		if (!ConfirmDiscardIfDirty())
		{
			return;
		}

		_editor.Text = string.Empty;
		CurrentFilePath = null;
		_editor.SetSyntaxHighlighting(null);
		IsDirty = false;
		ErrorMessage = string.Empty;
	}

	private void Open()
	{
		if (!ConfirmDiscardIfDirty())
		{
			return;
		}

		var filePath = _dialogService.ShowOpenDialog();
		if (filePath is null)
		{
			return;
		}

		try
		{
			_editor.Text = _fileService.ReadAllText(filePath);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			ErrorMessage = $"ファイルを開けませんでした: {ex.Message}";
			return;
		}

		CurrentFilePath = filePath;
		_editor.SetSyntaxHighlighting(filePath);
		IsDirty = false;
		ErrorMessage = string.Empty;
	}

	private bool Save()
	{
		if (CurrentFilePath is null)
		{
			return SaveAs();
		}

		try
		{
			_fileService.WriteAllText(CurrentFilePath, _editor.Text);
			IsDirty = false;
			ErrorMessage = string.Empty;
			return true;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			ErrorMessage = $"保存できませんでした: {ex.Message}";
			return false;
		}
	}

	private bool SaveAs()
	{
		var filePath = _dialogService.ShowSaveDialog(CurrentFilePath);
		if (filePath is null)
		{
			return false;
		}

		try
		{
			_fileService.WriteAllText(filePath, _editor.Text);
			CurrentFilePath = filePath;
			_editor.SetSyntaxHighlighting(filePath);
			IsDirty = false;
			ErrorMessage = string.Empty;
			return true;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			ErrorMessage = $"保存できませんでした: {ex.Message}";
			return false;
		}
	}

	/// <summary>
	/// 未保存の変更があれば確認する。続行してよい場合は<see langword="true"/>を返す。
	/// </summary>
	private bool ConfirmDiscardIfDirty()
	{
		if (!IsDirty)
		{
			return true;
		}

		return _unsavedChangesPrompt.Confirm() switch
		{
			true => Save(),
			false => true,
			null => false,
		};
	}
}
