namespace CountdownTimer.Tests;

/// <summary>
/// <see cref="CountdownEngine"/> の状態遷移・カウントダウン処理に関するテスト。
/// </summary>
public class CountdownEngineTests
{
	/// <summary>
	/// パス条件: 何も設定していない初期状態でStateがStoppedであること。
	/// </summary>
	[Fact]
	public void InitialState_IsStopped()
	{
		var engine = new CountdownEngine();

		Assert.Equal(CountdownState.Stopped, engine.State);
	}

	/// <summary>
	/// パス条件: 何も設定していない初期状態でRemainingTimeがゼロであること。
	/// </summary>
	[Fact]
	public void InitialRemainingTime_IsZero()
	{
		var engine = new CountdownEngine();

		Assert.Equal(TimeSpan.Zero, engine.RemainingTime);
	}

	/// <summary>
	/// パス条件: SetInitialTimeを呼ぶとRemainingTimeが設定した値になること。
	/// </summary>
	[Fact]
	public void SetInitialTime_WhenStopped_SetsRemainingTime()
	{
		var engine = new CountdownEngine();

		engine.SetInitialTime(TimeSpan.FromMinutes(5));

		Assert.Equal(TimeSpan.FromMinutes(5), engine.RemainingTime);
	}

	/// <summary>
	/// パス条件: Running中にSetInitialTimeを呼んでもRemainingTimeが変化しないこと。
	/// </summary>
	[Fact]
	public void SetInitialTime_WhenRunning_DoesNothing()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromMinutes(5));
		engine.Start();

		engine.SetInitialTime(TimeSpan.FromMinutes(10));

		Assert.Equal(TimeSpan.FromMinutes(5), engine.RemainingTime);
	}

	/// <summary>
	/// パス条件: 残り時間が設定された状態でStartを呼ぶとStateがRunningになること。
	/// </summary>
	[Fact]
	public void Start_WithRemainingTime_SetsStateToRunning()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromMinutes(5));

		engine.Start();

		Assert.Equal(CountdownState.Running, engine.State);
	}

	/// <summary>
	/// パス条件: 残り時間がゼロの状態でStartを呼んでもStateがRunningにならないこと。
	/// </summary>
	[Fact]
	public void Start_WithZeroRemainingTime_DoesNotStart()
	{
		var engine = new CountdownEngine();

		engine.Start();

		Assert.Equal(CountdownState.Stopped, engine.State);
	}

	/// <summary>
	/// パス条件: Running中にTickを呼ぶとRemainingTimeが1秒減ること。
	/// </summary>
	[Fact]
	public void Tick_WhenRunning_DecrementsOneSecond()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(10));
		engine.Start();

		engine.Tick();

		Assert.Equal(TimeSpan.FromSeconds(9), engine.RemainingTime);
	}

	/// <summary>
	/// パス条件: Stopped状態でTickを呼んでもRemainingTimeが変化しないこと。
	/// </summary>
	[Fact]
	public void Tick_WhenStopped_DoesNothing()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(10));

		engine.Tick();

		Assert.Equal(TimeSpan.FromSeconds(10), engine.RemainingTime);
	}

	/// <summary>
	/// パス条件: 残り1秒の状態でTickを呼ぶとRemainingTimeが0になりStateがCompletedになり、trueが返ること。
	/// </summary>
	[Fact]
	public void Tick_ReachingZero_SetsCompletedAndReturnsTrue()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(1));
		engine.Start();

		var result = engine.Tick();

		Assert.True(result);
		Assert.Equal(TimeSpan.Zero, engine.RemainingTime);
		Assert.Equal(CountdownState.Completed, engine.State);
	}

	/// <summary>
	/// パス条件: 完了に達しないTickはfalseを返すこと。
	/// </summary>
	[Fact]
	public void Tick_NotReachingZero_ReturnsFalse()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(10));
		engine.Start();

		var result = engine.Tick();

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: Running中にPauseを呼ぶとStateがPausedになること。
	/// </summary>
	[Fact]
	public void Pause_WhenRunning_SetsStateToPaused()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(10));
		engine.Start();

		engine.Pause();

		Assert.Equal(CountdownState.Paused, engine.State);
	}

	/// <summary>
	/// パス条件: Paused状態でStartを呼ぶとStateがRunningに戻り、Tickでカウントダウンが再開されること。
	/// </summary>
	[Fact]
	public void Start_AfterPause_ResumesCountingDown()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(10));
		engine.Start();
		engine.Tick();
		engine.Pause();

		engine.Start();
		engine.Tick();

		Assert.Equal(CountdownState.Running, engine.State);
		Assert.Equal(TimeSpan.FromSeconds(8), engine.RemainingTime);
	}

	/// <summary>
	/// パス条件: カウントダウン中にResetを呼ぶと、RemainingTimeが初期値に戻りStateがStoppedになること。
	/// </summary>
	[Fact]
	public void Reset_RestoresInitialTimeAndStoppedState()
	{
		var engine = new CountdownEngine();
		engine.SetInitialTime(TimeSpan.FromSeconds(10));
		engine.Start();
		engine.Tick();
		engine.Tick();

		engine.Reset();

		Assert.Equal(TimeSpan.FromSeconds(10), engine.RemainingTime);
		Assert.Equal(CountdownState.Stopped, engine.State);
	}
}
