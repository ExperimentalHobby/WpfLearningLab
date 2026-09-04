namespace ParallelImageProcessor.Models;

/// <summary>
/// バッチ画像処理の設定。
/// </summary>
/// <param name="ResizeEnabled">リサイズを行うかどうか。</param>
/// <param name="TargetWidth">リサイズ後の幅(px)。</param>
/// <param name="TargetHeight">リサイズ後の高さ(px)。</param>
/// <param name="GrayscaleEnabled">グレースケール化を行うかどうか。</param>
public record ImageProcessingOptions(bool ResizeEnabled, int TargetWidth, int TargetHeight, bool GrayscaleEnabled);
