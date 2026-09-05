namespace MemoryLeakLab.ViewModels;

/// <summary>
/// 追跡対象オブジェクトを<see cref="WeakReference"/>で保持し、生存数を計測するヘルパー。
/// 対象への強参照は持たないため、追跡すること自体がGCを妨げることはない。
/// </summary>
public class LeakTracker
{
	private readonly List<WeakReference> _tracked = [];

	/// <summary>
	/// これまでに <see cref="Track"/> で登録した総数。
	/// </summary>
	public int TotalCount => _tracked.Count;

	/// <summary>
	/// 指定したオブジェクトを弱参照として追跡対象に加える。
	/// </summary>
	/// <param name="target">追跡対象。</param>
	public void Track(object target) => _tracked.Add(new WeakReference(target));

	/// <summary>
	/// 現時点で生存している(GCされていない)追跡対象の数を返す。
	/// </summary>
	public int CountAlive() => _tracked.Count(wr => wr.IsAlive);
}
