using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GaugeControlLib;

/// <summary>
/// 値を円弧状のゲージ(メーター)で表示するカスタムコントロール。
/// 外観は<c>Themes/Generic.xaml</c>のControlTemplateで定義する(<see cref="Control"/>を継承)。
/// </summary>
public class GaugeControl : Control
{
	/// <summary>ゲージが示す現在値。</summary>
	public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
		nameof(Value), typeof(double), typeof(GaugeControl),
		new FrameworkPropertyMetadata(0.0, OnValueChanged));

	/// <summary>ゲージの最小値。</summary>
	public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
		nameof(Minimum), typeof(double), typeof(GaugeControl), new PropertyMetadata(0.0));

	/// <summary>ゲージの最大値。</summary>
	public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
		nameof(Maximum), typeof(double), typeof(GaugeControl), new PropertyMetadata(100.0));

	/// <summary>
	/// <see cref="ThresholdExceeded"/>イベントを発火させるしきい値。<see cref="double.NaN"/>の場合は判定しない。
	/// </summary>
	public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register(
		nameof(Threshold), typeof(double), typeof(GaugeControl), new PropertyMetadata(double.NaN));

	/// <summary>
	/// 針の現在の表示角度(度)。<see cref="Value"/>変更時にアニメーションで追従する、ControlTemplate専用の内部状態。
	/// </summary>
	public static readonly DependencyProperty AnimatedAngleProperty = DependencyProperty.Register(
		nameof(AnimatedAngle), typeof(double), typeof(GaugeControl), new PropertyMetadata(GaugeMath.StartAngle));

	/// <summary>
	/// <see cref="Value"/>が下から上へ<see cref="Threshold"/>を超えたときに発火するルーテッドイベント。
	/// </summary>
	public static readonly RoutedEvent ThresholdExceededEvent = EventManager.RegisterRoutedEvent(
		nameof(ThresholdExceeded), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GaugeControl));

	static GaugeControl()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(GaugeControl), new FrameworkPropertyMetadata(typeof(GaugeControl)));
	}

	/// <summary>ゲージが示す現在値。</summary>
	public double Value
	{
		get => (double)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	/// <summary>ゲージの最小値。</summary>
	public double Minimum
	{
		get => (double)GetValue(MinimumProperty);
		set => SetValue(MinimumProperty, value);
	}

	/// <summary>ゲージの最大値。</summary>
	public double Maximum
	{
		get => (double)GetValue(MaximumProperty);
		set => SetValue(MaximumProperty, value);
	}

	/// <summary>
	/// <see cref="ThresholdExceeded"/>イベントを発火させるしきい値。<see cref="double.NaN"/>の場合は判定しない。
	/// </summary>
	public double Threshold
	{
		get => (double)GetValue(ThresholdProperty);
		set => SetValue(ThresholdProperty, value);
	}

	/// <summary>針の現在の表示角度(度)。ControlTemplate内のRotateTransformがバインドする。</summary>
	public double AnimatedAngle => (double)GetValue(AnimatedAngleProperty);

	/// <summary>
	/// <see cref="Value"/>が下から上へ<see cref="Threshold"/>を超えたときに発火する。
	/// </summary>
	public event RoutedEventHandler ThresholdExceeded
	{
		add => AddHandler(ThresholdExceededEvent, value);
		remove => RemoveHandler(ThresholdExceededEvent, value);
	}

	private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var gauge = (GaugeControl)d;
		var oldValue = (double)e.OldValue;
		var newValue = (double)e.NewValue;

		gauge.AnimateNeedleTo(GaugeMath.ValueToAngle(newValue, gauge.Minimum, gauge.Maximum));

		if (!double.IsNaN(gauge.Threshold) && GaugeMath.HasCrossedThresholdUpward(oldValue, newValue, gauge.Threshold))
		{
			gauge.RaiseEvent(new RoutedEventArgs(ThresholdExceededEvent, gauge));
		}
	}

	private void AnimateNeedleTo(double targetAngle)
	{
		var animation = new DoubleAnimation(targetAngle, TimeSpan.FromMilliseconds(400))
		{
			EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
		};
		BeginAnimation(AnimatedAngleProperty, animation);
	}

	/// <inheritdoc/>
	// Controlは既定ではOnCreateAutomationPeerをオーバーライドしないため、明示的にオーバーライドしないと
	// UI Automationのツリーに一切現れず(AutomationProperties.AutomationIdを設定しても検出できない)、
	// アクセシビリティツールやテスト自動化から参照できない。
	protected override AutomationPeer OnCreateAutomationPeer() => new GaugeControlAutomationPeer(this);
}

/// <summary>
/// <see cref="GaugeControl"/>をUI Automationのツリーに公開するための最小限の<see cref="AutomationPeer"/>。
/// </summary>
internal class GaugeControlAutomationPeer : FrameworkElementAutomationPeer
{
	public GaugeControlAutomationPeer(GaugeControl owner) : base(owner)
	{
	}

	protected override string GetClassNameCore() => nameof(GaugeControl);

	protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Custom;
}
