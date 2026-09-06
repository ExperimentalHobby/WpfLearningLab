namespace MusicPlayer.Services;

/// <summary>
/// 実際のメディア再生(<see cref="System.Windows.Controls.MediaElement"/>)への操作の抽象。
/// ViewModelをWPFのメディア再生パイプラインから独立させ、テスト可能にする。
/// </summary>
public interface IMediaPlayerController
{
	/// <summary>現在の再生位置。</summary>
	TimeSpan Position { get; set; }

	/// <summary>読み込み済みメディアの長さ。未読込の場合は<see langword="null"/>。</summary>
	TimeSpan? Duration { get; }

	/// <summary>
	/// メディアの再生が末尾まで終了したときに発火する。
	/// </summary>
	event EventHandler? MediaEnded;

	/// <summary>
	/// メディアの読み込みが完了し、<see cref="Duration"/>が確定したときに発火する。
	/// </summary>
	event EventHandler? MediaOpened;

	/// <summary>
	/// メディアの読み込み・再生に失敗したときに発火する(存在しないファイル・破損ファイルなど)。
	/// </summary>
	event EventHandler<Exception?>? MediaFailed;

	/// <summary>
	/// 指定した音声ファイルを読み込み、再生を開始する。
	/// </summary>
	void Load(string filePath);

	/// <summary>再生を再開する。</summary>
	void Play();

	/// <summary>再生を一時停止する。</summary>
	void Pause();

	/// <summary>再生を停止し、再生位置を先頭に戻す。</summary>
	void Stop();
}
