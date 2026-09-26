namespace AnimatedDashboard.Models;

/// <summary>
/// カウントアップアニメーションに使うイージング関数の種類。
/// </summary>
public enum EasingType
{
	/// <summary>等速。</summary>
	Linear,

	/// <summary>開始時にゆっくり加速する。</summary>
	EaseIn,

	/// <summary>終了時にゆっくり減速する。</summary>
	EaseOut,

	/// <summary>終端で跳ねるように動く。</summary>
	Bounce,

	/// <summary>ゴム紐のように伸縮しながら動く。</summary>
	Elastic,
}
