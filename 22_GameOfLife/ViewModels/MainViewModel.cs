using GameOfLife.Services;

namespace GameOfLife.ViewModels;

/// <summary>
/// ライフゲームアプリのメイン画面のViewModel。盤面の実行状態(再生/停止・世代・速度)を管理する。
/// 盤面の描画とマウス操作によるセル配置はView層(コードビハインド)が担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private const int MinIntervalMilliseconds = 50;
	private const int MaxIntervalMilliseconds = 2000;

	private int _generation;
	private bool _isRunning;
	private int _intervalMilliseconds = 300;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="width">盤面の幅(セル数)。</param>
	/// <param name="height">盤面の高さ(セル数)。</param>
	public MainViewModel(int width, int height)
	{
		Engine = new GameOfLifeEngine(width, height);

		PlayPauseCommand = new RelayCommand(() => IsRunning = !IsRunning);
		ResetCommand = new RelayCommand(Reset);
	}

	/// <summary>盤面の状態を保持するルールエンジン。View層はこれを直接参照して描画する。</summary>
	public GameOfLifeEngine Engine { get; }

	/// <summary>
	/// 盤面の内容が変化した(1世代進んだ、またはリセットされた)ときに発火する。
	/// <see cref="Generation"/>が0のままリセットされた場合も再描画が必要なため、
	/// <see cref="ObservableObject.PropertyChanged"/>とは別にこのイベントを用意している。
	/// </summary>
	public event EventHandler? BoardChanged;

	/// <summary>現在の世代数。</summary>
	public int Generation
	{
		get => _generation;
		private set => SetProperty(ref _generation, value);
	}

	/// <summary>自動進行中かどうか。</summary>
	public bool IsRunning
	{
		get => _isRunning;
		private set => SetProperty(ref _isRunning, value);
	}

	/// <summary>自動進行の間隔(ミリ秒)。<see cref="MinIntervalMilliseconds"/>〜<see cref="MaxIntervalMilliseconds"/>にクランプされる。</summary>
	public int IntervalMilliseconds
	{
		get => _intervalMilliseconds;
		set => SetProperty(ref _intervalMilliseconds, Math.Clamp(value, MinIntervalMilliseconds, MaxIntervalMilliseconds));
	}

	/// <summary>再生/一時停止を切り替えるコマンド。</summary>
	public RelayCommand PlayPauseCommand { get; }

	/// <summary>盤面をクリアし世代・実行状態をリセットするコマンド。</summary>
	public RelayCommand ResetCommand { get; }

	/// <summary>
	/// 盤面を1世代進める。<see cref="System.Windows.Threading.DispatcherTimer"/>のTickからView層が呼び出す。
	/// </summary>
	public void AdvanceGeneration()
	{
		Engine.AdvanceGeneration();
		Generation++;
		BoardChanged?.Invoke(this, EventArgs.Empty);
	}

	private void Reset()
	{
		Engine.Clear();
		Generation = 0;
		IsRunning = false;
		BoardChanged?.Invoke(this, EventArgs.Empty);
	}
}
