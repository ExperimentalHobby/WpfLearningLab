using System.Windows.Media;
using PaintTool.Services;

namespace PaintTool.ViewModels;

/// <summary>
/// お絵かきツールのメイン画面のViewModel。ペン設定・Undo/Redo・保存を<see cref="IInkCanvasController"/>越しに操作する。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IInkCanvasController _controller;
	private readonly ISaveFileDialogService _saveDialog;

	private Color _penColor = Colors.Black;
	private double _penWidth = 3.0;
	private bool _isEraserMode;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="controller">描画キャンバスへの操作を担うコントローラー。</param>
	/// <param name="saveDialog">保存先パスの選択を担うダイアログサービス。</param>
	public MainViewModel(IInkCanvasController controller, ISaveFileDialogService saveDialog)
	{
		_controller = controller;
		_saveDialog = saveDialog;

		UndoCommand = new RelayCommand(_controller.Undo, () => _controller.CanUndo);
		RedoCommand = new RelayCommand(_controller.Redo, () => _controller.CanRedo);
		ClearAllCommand = new RelayCommand(_controller.ClearAll);
		SaveCommand = new RelayCommand(Save);
		SelectColorCommand = new RelayCommand<string>(SelectColor);

		_controller.StateChanged += (_, _) =>
		{
			UndoCommand.RaiseCanExecuteChanged();
			RedoCommand.RaiseCanExecuteChanged();
		};
	}

	/// <summary>現在のペンの色。</summary>
	public Color PenColor
	{
		get => _penColor;
		set
		{
			if (SetProperty(ref _penColor, value))
			{
				_controller.SetPenColor(value);
			}
		}
	}

	/// <summary>現在のペンの太さ。</summary>
	public double PenWidth
	{
		get => _penWidth;
		set
		{
			if (SetProperty(ref _penWidth, value))
			{
				_controller.SetPenWidth(value);
			}
		}
	}

	/// <summary>消しゴムモードが有効かどうか。</summary>
	public bool IsEraserMode
	{
		get => _isEraserMode;
		set
		{
			if (SetProperty(ref _isEraserMode, value))
			{
				_controller.SetEraserMode(value);
			}
		}
	}

	/// <summary>直前の操作を取り消すコマンド。</summary>
	public RelayCommand UndoCommand { get; }

	/// <summary>取り消した操作をやり直すコマンド。</summary>
	public RelayCommand RedoCommand { get; }

	/// <summary>描画内容を全て消去するコマンド。</summary>
	public RelayCommand ClearAllCommand { get; }

	/// <summary>描画内容をPNGファイルとして保存するコマンド。</summary>
	public RelayCommand SaveCommand { get; }

	/// <summary>色名(例: "Red")を指定してペンの色を切り替えるコマンド。</summary>
	public RelayCommand<string> SelectColorCommand { get; }

	private void Save()
	{
		var path = _saveDialog.PromptForSavePath("png", "PNG画像 (*.png)|*.png");
		if (path is null)
		{
			return;
		}

		_controller.SaveAsPng(path);
	}

	private void SelectColor(string? colorName)
	{
		if (string.IsNullOrEmpty(colorName))
		{
			return;
		}

		PenColor = (Color)ColorConverter.ConvertFromString(colorName);
	}
}
