using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AnimatedDashboard.Models;
using AnimatedDashboard.Services;

namespace AnimatedDashboard.Views;

/// <summary>
/// KPI値を大きな数字でカウントアップ表示するカード。
/// <see cref="TargetValue"/>が変わると、実際に<see cref="Storyboard"/>+<see cref="DoubleAnimation"/>を
/// 組み立てて<see cref="DisplayValue"/>をアニメーションさせる。
/// </summary>
public partial class KpiCard : UserControl
{
	/// <summary>表示すべき実際の値。</summary>
	public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register(
		nameof(TargetValue), typeof(double), typeof(KpiCard), new PropertyMetadata(0.0, OnTargetValueChanged));

	/// <summary>
	/// アニメーション中の表示値。<see cref="Storyboard"/>のアニメーション対象となる内部状態。
	/// </summary>
	public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(
		nameof(DisplayValue), typeof(double), typeof(KpiCard), new PropertyMetadata(0.0));

	/// <summary>カウントアップアニメーションに使うイージング関数の種類。</summary>
	public static readonly DependencyProperty EasingProperty = DependencyProperty.Register(
		nameof(Easing), typeof(EasingType), typeof(KpiCard), new PropertyMetadata(EasingType.EaseOut));

	/// <summary>指標名。</summary>
	public static readonly DependencyProperty MetricNameProperty = DependencyProperty.Register(
		nameof(MetricName), typeof(string), typeof(KpiCard), new PropertyMetadata(string.Empty));

	/// <summary>単位。</summary>
	public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
		nameof(Unit), typeof(string), typeof(KpiCard), new PropertyMetadata(string.Empty));

	public KpiCard()
	{
		InitializeComponent();
	}

	/// <summary>表示すべき実際の値。</summary>
	public double TargetValue
	{
		get => (double)GetValue(TargetValueProperty);
		set => SetValue(TargetValueProperty, value);
	}

	/// <summary>
	/// アニメーション中の表示値。値は<see cref="Storyboard"/>経由でのみ変更され、
	/// このプロパティ自体に書き込みsetterを持たせるとStoryboard実行中は反映されず紛らわしいため、
	/// 取得専用にしている(34_CustomGaugeControlの<c>GaugeControl.AnimatedAngle</c>と同様)。
	/// </summary>
	public double DisplayValue => (double)GetValue(DisplayValueProperty);

	/// <summary>カウントアップアニメーションに使うイージング関数の種類。</summary>
	public EasingType Easing
	{
		get => (EasingType)GetValue(EasingProperty);
		set => SetValue(EasingProperty, value);
	}

	/// <summary>指標名。</summary>
	public string MetricName
	{
		get => (string)GetValue(MetricNameProperty);
		set => SetValue(MetricNameProperty, value);
	}

	/// <summary>単位。</summary>
	public string Unit
	{
		get => (string)GetValue(UnitProperty);
		set => SetValue(UnitProperty, value);
	}

	private static void OnTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var card = (KpiCard)d;
		card.AnimateTo((double)e.NewValue);
	}

	private void AnimateTo(double target)
	{
		var animation = new DoubleAnimation
		{
			From = DisplayValue,
			To = target,
			Duration = TimeSpan.FromMilliseconds(900),
			EasingFunction = EasingFunctionFactory.Create(Easing),
		};

		var storyboard = new Storyboard();
		storyboard.Children.Add(animation);
		Storyboard.SetTarget(animation, this);
		Storyboard.SetTargetProperty(animation, new PropertyPath(DisplayValueProperty));

		storyboard.Begin(this);
	}

	/// <inheritdoc/>
	// UserControlも既定ではOnCreateAutomationPeerをオーバーライドしないため、明示的にオーバーライドしないと
	// AutomationProperties.AutomationIdを設定してもUI Automationのツリーに現れない
	// (34_CustomGaugeControlのGaugeControlで見つかった問題と同様)。
	protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);
}
