using System.Windows.Input;
using MemoryLeakLab.Services;

namespace MemoryLeakLab.ViewModels;

/// <summary>
/// メモリリーク検証ラボのメイン画面ViewModel。
/// <see cref="LeakMode.Bad"/>(強参照購読)と<see cref="LeakMode.Good"/>(弱参照購読)を切り替えて
/// 生成・参照解放・GC実行を行い、生存数の違いを確認できるようにする。
/// </summary>
public class MainViewModel : ObservableObject
{
	private const int GenerateCount = 10;

	private readonly EventPublisher _publisher = new();
	private readonly LeakTracker _tracker = new();

	/// <summary>
	/// <see cref="ReleaseReferencesCommand"/>で参照を切るための強参照リスト。
	/// このリストが購読者への唯一の外部強参照であり、Clearすることで参照が切れる。
	/// </summary>
	private readonly List<object> _subscribers = [];

	private LeakMode _mode = LeakMode.Bad;

	/// <summary>
	/// 現在のリーク再現モード。
	/// </summary>
	public LeakMode Mode
	{
		get => _mode;
		set => SetProperty(ref _mode, value);
	}

	/// <summary>
	/// これまでに生成し追跡対象に加えた総数。
	/// </summary>
	public int TotalCount => _tracker.TotalCount;

	/// <summary>
	/// 現時点で生存している(GCされていない)追跡対象の数。
	/// </summary>
	public int AliveCount => _tracker.CountAlive();

	/// <summary>
	/// 現在のモードに応じた購読者を<see cref="GenerateCount"/>件生成するコマンド。
	/// </summary>
	public ICommand GenerateCommand { get; }

	/// <summary>
	/// 生成済み購読者への強参照をすべて解放するコマンド。
	/// </summary>
	public ICommand ReleaseReferencesCommand { get; }

	/// <summary>
	/// GCを強制実行し、生存数を再計測するコマンド。
	/// </summary>
	public ICommand CollectGarbageCommand { get; }

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel()
	{
		GenerateCommand = new RelayCommand(Generate);
		ReleaseReferencesCommand = new RelayCommand(ReleaseReferences);
		CollectGarbageCommand = new RelayCommand(CollectGarbage);
	}

	private void Generate()
	{
		for (var i = 0; i < GenerateCount; i++)
		{
			object subscriber = Mode == LeakMode.Bad
				? new LeakySubscriberViewModel(_publisher)
				: new WeakSubscriberViewModel(_publisher);

			_subscribers.Add(subscriber);
			_tracker.Track(subscriber);
		}

		OnPropertyChanged(nameof(TotalCount));
		OnPropertyChanged(nameof(AliveCount));
	}

	private void ReleaseReferences()
	{
		_subscribers.Clear();
		OnPropertyChanged(nameof(AliveCount));
	}

	private void CollectGarbage()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
		OnPropertyChanged(nameof(AliveCount));
	}
}
