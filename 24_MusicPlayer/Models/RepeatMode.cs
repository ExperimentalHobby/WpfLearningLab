namespace MusicPlayer.Models;

/// <summary>
/// プレイリストのリピート再生モード。
/// </summary>
public enum RepeatMode
{
	/// <summary>リピートしない。最後の曲の再生が終わったら停止する。</summary>
	Off,

	/// <summary>現在の曲だけを繰り返す。</summary>
	One,

	/// <summary>プレイリスト全体を繰り返す。</summary>
	All,
}
