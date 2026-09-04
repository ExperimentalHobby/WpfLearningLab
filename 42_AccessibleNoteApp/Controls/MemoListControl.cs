using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AccessibleNoteApp.Models;
using AccessibleNoteApp.Services;

namespace AccessibleNoteApp.Controls;

/// <summary>
/// メモ一覧を表示する自前描画コントロール。<see cref="System.Windows.Controls.ItemsControl"/>や
/// <see cref="System.Windows.Controls.ListBox"/>を使わず<see cref="OnRender"/>で直接描画することで、
/// 既製コントロールが標準で持つUI Automationサポートに頼らず、<see cref="MemoListControlAutomationPeer"/>/
/// <see cref="MemoListItemAutomationPeer"/>を自作してツリーに公開する学習用の構成にしている。
/// 色は全て<see cref="SystemColors"/>のシステムカラーを使うため、Windowsのハイコントラストテーマにも
/// 自動的に追従する。
/// </summary>
public class MemoListControl : Control
{
	/// <summary>1行あたりの高さ(ピクセル)。</summary>
	private const double RowHeight = 28;

	/// <summary><see cref="ItemsSource"/>依存関係プロパティ。</summary>
	public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
		nameof(ItemsSource),
		typeof(IReadOnlyList<Memo>),
		typeof(MemoListControl),
		new FrameworkPropertyMetadata(Array.Empty<Memo>(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

	/// <summary><see cref="SelectedIndex"/>依存関係プロパティ。</summary>
	public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
		nameof(SelectedIndex),
		typeof(int),
		typeof(MemoListControl),
		new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexChanged));

	/// <summary>Enter/Space/ダブルクリックで項目が「実行」された(編集を開始する)ことを表すルーテッドイベント。</summary>
	public static readonly RoutedEvent ItemActivatedEvent = EventManager.RegisterRoutedEvent(
		nameof(ItemActivated), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MemoListControl));

	/// <summary>
	/// 選択中の項目に対しDeleteキーが押されたことを表すルーテッドイベント。
	/// 本文入力欄でのテキスト編集用Deleteキーと衝突しないよう、一覧コントロールがフォーカスを
	/// 持つときのみ発火する。
	/// </summary>
	public static readonly RoutedEvent DeleteRequestedEvent = EventManager.RegisterRoutedEvent(
		nameof(DeleteRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MemoListControl));

	static MemoListControl()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(MemoListControl), new FrameworkPropertyMetadata(typeof(MemoListControl)));
		FocusableProperty.OverrideMetadata(typeof(MemoListControl), new FrameworkPropertyMetadata(true));
	}

	/// <summary>表示するメモの一覧。</summary>
	public IReadOnlyList<Memo> ItemsSource
	{
		get => (IReadOnlyList<Memo>)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	/// <summary>選択中の項目のIndex(未選択の場合は-1)。</summary>
	public int SelectedIndex
	{
		get => (int)GetValue(SelectedIndexProperty);
		set => SetValue(SelectedIndexProperty, value);
	}

	/// <summary><see cref="ItemsSource"/>の件数。</summary>
	public int ItemCount => ItemsSource.Count;

	/// <summary>項目が「実行」されたときに発火するイベント。</summary>
	public event RoutedEventHandler ItemActivated
	{
		add => AddHandler(ItemActivatedEvent, value);
		remove => RemoveHandler(ItemActivatedEvent, value);
	}

	/// <summary>選択中の項目に対しDeleteキーが押されたときに発火するイベント。</summary>
	public event RoutedEventHandler DeleteRequested
	{
		add => AddHandler(DeleteRequestedEvent, value);
		remove => RemoveHandler(DeleteRequestedEvent, value);
	}

	/// <summary>
	/// 指定したIndexのメモのタイトルを返す。範囲外の場合は<see langword="null"/>。
	/// </summary>
	public string? GetItemTitle(int index) =>
		index >= 0 && index < ItemsSource.Count ? ItemsSource[index].Title : null;

	/// <summary>
	/// 指定したIndexの行の、コントロールのローカル座標系での矩形を返す。
	/// </summary>
	public Rect GetItemLocalBounds(int index) => new(0, index * RowHeight, ActualWidth, RowHeight);

	/// <summary>
	/// 指定したIndexの行の、画面座標系での矩形を返す(<see cref="MemoListItemAutomationPeer"/>から使う)。
	/// </summary>
	public Rect GetItemScreenBounds(int index)
	{
		var local = GetItemLocalBounds(index);
		if (!IsLoaded || PresentationSource.FromVisual(this) is null)
		{
			return local;
		}

		var topLeft = PointToScreen(local.TopLeft);
		return new Rect(topLeft, local.Size);
	}

	/// <inheritdoc/>
	protected override AutomationPeer OnCreateAutomationPeer() => new MemoListControlAutomationPeer(this);

	/// <inheritdoc/>
	protected override Size MeasureOverride(Size constraint)
	{
		var width = double.IsPositiveInfinity(constraint.Width) ? 200 : constraint.Width;
		return new Size(width, RowHeight * ItemsSource.Count);
	}

	/// <inheritdoc/>
	protected override void OnRender(DrawingContext drawingContext)
	{
		drawingContext.DrawRectangle(SystemColors.WindowBrush, null, new Rect(RenderSize));

		var dpi = VisualTreeHelper.GetDpi(this);
		for (var i = 0; i < ItemsSource.Count; i++)
		{
			var rowBounds = GetItemLocalBounds(i);
			var isSelected = i == SelectedIndex;
			if (isSelected)
			{
				var background = IsKeyboardFocused ? SystemColors.HighlightBrush : SystemColors.ControlBrush;
				drawingContext.DrawRectangle(background, null, rowBounds);
			}

			var title = ItemsSource[i].Title;
			var text = new FormattedText(
				string.IsNullOrWhiteSpace(title) ? "(無題)" : title,
				CultureInfo.CurrentUICulture,
				FlowDirection.LeftToRight,
				new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
				FontSize,
				isSelected ? SystemColors.HighlightTextBrush : SystemColors.ControlTextBrush,
				dpi.PixelsPerDip);
			drawingContext.DrawText(text, new Point(rowBounds.X + 4, rowBounds.Y + (RowHeight - text.Height) / 2));
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		base.OnMouseLeftButtonDown(e);
		Focus();

		var index = (int)(e.GetPosition(this).Y / RowHeight);
		if (index < 0 || index >= ItemsSource.Count)
		{
			return;
		}

		SelectedIndex = index;
		if (e.ClickCount >= 2)
		{
			RaiseEvent(new RoutedEventArgs(ItemActivatedEvent));
		}
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);

		if (e.Key is Key.Enter or Key.Space)
		{
			if (SelectedIndex >= 0)
			{
				RaiseEvent(new RoutedEventArgs(ItemActivatedEvent));
			}
			e.Handled = true;
			return;
		}

		if (e.Key == Key.Delete)
		{
			if (SelectedIndex >= 0)
			{
				RaiseEvent(new RoutedEventArgs(DeleteRequestedEvent));
			}
			e.Handled = true;
			return;
		}

		var nextIndex = MemoListNavigator.GetNextIndex(SelectedIndex, ItemsSource.Count, e.Key);
		if (nextIndex is int index)
		{
			SelectedIndex = index;
			e.Handled = true;
		}
	}

	private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var control = (MemoListControl)d;
		if (UIElementAutomationPeer.FromElement(control) is not MemoListControlAutomationPeer peer)
		{
			return;
		}

		var newIndex = (int)e.NewValue;
		if (newIndex >= 0)
		{
			peer.CreateItemPeer(newIndex).RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
		}
	}
}
