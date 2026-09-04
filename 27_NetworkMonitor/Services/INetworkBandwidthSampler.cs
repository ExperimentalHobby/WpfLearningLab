namespace NetworkMonitor.Services;

/// <summary>
/// ネットワークインターフェースの送受信帯域を計測する処理の抽象。
/// </summary>
public interface INetworkBandwidthSampler
{
	/// <summary>
	/// 計測可能なネットワークインターフェースのインスタンス名一覧を取得する。
	/// </summary>
	IReadOnlyList<string> GetInstanceNames();

	/// <summary>
	/// 指定したインターフェースの現在の送受信バイト/秒を計測する。
	/// </summary>
	/// <param name="instanceName">計測対象のインターフェースのインスタンス名。</param>
	(double SentBytesPerSec, double ReceivedBytesPerSec) Sample(string instanceName);
}
