namespace MemoryLeakLab.ViewModels;

/// <summary>
/// メモリリークの再現モード。
/// </summary>
public enum LeakMode
{
	/// <summary>Bad版。強参照購読で解除を行わず、リークを再現する。</summary>
	Bad,

	/// <summary>Good版。WeakEventManagerで弱参照購読し、リークを解消する。</summary>
	Good,
}
