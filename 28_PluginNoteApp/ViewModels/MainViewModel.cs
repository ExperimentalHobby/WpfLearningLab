using System.Collections.ObjectModel;
using PluginNoteApp.Contracts;
using PluginNoteApp.Services;

namespace PluginNoteApp.ViewModels;

/// <summary>
/// メモ入力・プラグイン選択/実行・プラグイン読込エラー表示を行うメイン画面のViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private string _memoText = string.Empty;
	private IMemoPlugin? _selectedPlugin;
	private string _pluginOutput = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。コンストラクタ内でプラグインフォルダを走査し、読み込む。
	/// </summary>
	/// <param name="pluginLoader">プラグイン読込処理。</param>
	/// <param name="pluginDirectory">プラグインDLLが置かれているフォルダのパス。</param>
	public MainViewModel(IPluginLoader pluginLoader, string pluginDirectory)
	{
		RunPluginCommand = new RelayCommand(RunPlugin, CanRunPlugin);

		foreach (var result in pluginLoader.LoadPlugins(pluginDirectory))
		{
			if (result.Success)
			{
				Plugins.Add(result.Plugin!);
			}
			else
			{
				LoadErrors.Add($"{result.PluginName}: {result.ErrorMessage}");
			}
		}
	}

	/// <summary>メモ本文。</summary>
	public string MemoText
	{
		get => _memoText;
		set => SetProperty(ref _memoText, value);
	}

	/// <summary>読み込みに成功したプラグイン一覧。</summary>
	public ObservableCollection<IMemoPlugin> Plugins { get; } = [];

	/// <summary>読み込みに失敗したDLLのエラーメッセージ一覧。</summary>
	public ObservableCollection<string> LoadErrors { get; } = [];

	/// <summary>選択中のプラグイン。</summary>
	public IMemoPlugin? SelectedPlugin
	{
		get => _selectedPlugin;
		set
		{
			if (SetProperty(ref _selectedPlugin, value))
			{
				RunPluginCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>選択中プラグインの実行結果。</summary>
	public string PluginOutput
	{
		get => _pluginOutput;
		private set => SetProperty(ref _pluginOutput, value);
	}

	/// <summary>選択中のプラグインでメモ本文を処理するコマンド。</summary>
	public RelayCommand RunPluginCommand { get; }

	private bool CanRunPlugin() => SelectedPlugin is not null;

	private void RunPlugin()
	{
		try
		{
			PluginOutput = SelectedPlugin!.Process(MemoText);
		}
		catch (Exception ex)
		{
			// プラグインは任意の外部コードであり、どんな例外を投げるか予測できない。
			// ここで捕捉し損ねるとプラグイン1つの不具合でホストアプリ全体がクラッシュしてしまう
			// (プラグイン機構として致命的なため、意図的に広く捕捉する)。
			PluginOutput = $"プラグインの実行中にエラーが発生しました: {ex.Message}";
		}
	}
}
