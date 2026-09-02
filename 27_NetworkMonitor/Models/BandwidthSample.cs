namespace NetworkMonitor.Models;

/// <summary>
/// ある時点でのネットワーク送受信帯域の計測値。
/// </summary>
/// <param name="Timestamp">計測日時。</param>
/// <param name="SentBytesPerSec">送信バイト/秒。</param>
/// <param name="ReceivedBytesPerSec">受信バイト/秒。</param>
public record BandwidthSample(DateTime Timestamp, double SentBytesPerSec, double ReceivedBytesPerSec);
