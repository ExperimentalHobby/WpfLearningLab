using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using KanbanTaskManager.Models;
using KanbanTaskManager.ViewModels;

namespace KanbanTaskManager.Behaviors;

/// <summary>
/// WPFのDragDrop API(マウスイベント・<see cref="DragDrop"/>)をICommandに橋渡しする添付ビヘイビア。
/// コードビハインドやView側にドラッグ&ドロップのロジックを書かせないために使う。
/// <list type="bullet">
/// <item><see cref="IsDragSourceProperty"/>: タスク一覧の<c>ItemsControl</c>に設定し、ドラッグ開始を検知する。</item>
/// <item><see cref="DropCommandProperty"/> / <see cref="DropTargetStatusProperty"/>: ドロップ先のコンテナに設定し、
/// ドロップされたタスクと移動先の状態を<see cref="MoveTaskRequest"/>としてコマンドへ渡す。</item>
/// </list>
/// </summary>
public static class DragDropBehavior
{
	private static Point _dragStartPosition;

	/// <summary>
	/// このコントロールをドラッグ元として扱うかどうか。
	/// </summary>
	public static readonly DependencyProperty IsDragSourceProperty =
		DependencyProperty.RegisterAttached(
			"IsDragSource",
			typeof(bool),
			typeof(DragDropBehavior),
			new PropertyMetadata(false, OnIsDragSourceChanged));

	/// <summary>ドロップされたときに実行するコマンド。パラメータは <see cref="MoveTaskRequest"/>。</summary>
	public static readonly DependencyProperty DropCommandProperty =
		DependencyProperty.RegisterAttached(
			"DropCommand",
			typeof(ICommand),
			typeof(DragDropBehavior),
			new PropertyMetadata(null, OnDropCommandChanged));

	/// <summary>このコンテナにドロップされたタスクの移動先の状態。</summary>
	public static readonly DependencyProperty DropTargetStatusProperty =
		DependencyProperty.RegisterAttached(
			"DropTargetStatus",
			typeof(KanbanStatus),
			typeof(DragDropBehavior),
			new PropertyMetadata(KanbanStatus.Todo));

	/// <summary><see cref="IsDragSourceProperty"/> の値を取得する。</summary>
	public static bool GetIsDragSource(DependencyObject obj) => (bool)obj.GetValue(IsDragSourceProperty);

	/// <summary><see cref="IsDragSourceProperty"/> の値を設定する。</summary>
	public static void SetIsDragSource(DependencyObject obj, bool value) => obj.SetValue(IsDragSourceProperty, value);

	/// <summary><see cref="DropCommandProperty"/> の値を取得する。</summary>
	public static ICommand? GetDropCommand(DependencyObject obj) => (ICommand?)obj.GetValue(DropCommandProperty);

	/// <summary><see cref="DropCommandProperty"/> の値を設定する。</summary>
	public static void SetDropCommand(DependencyObject obj, ICommand? value) => obj.SetValue(DropCommandProperty, value);

	/// <summary><see cref="DropTargetStatusProperty"/> の値を取得する。</summary>
	public static KanbanStatus GetDropTargetStatus(DependencyObject obj) => (KanbanStatus)obj.GetValue(DropTargetStatusProperty);

	/// <summary><see cref="DropTargetStatusProperty"/> の値を設定する。</summary>
	public static void SetDropTargetStatus(DependencyObject obj, KanbanStatus value) => obj.SetValue(DropTargetStatusProperty, value);

	private static void OnIsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not UIElement element)
		{
			return;
		}

		if ((bool)e.NewValue)
		{
			element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
			element.PreviewMouseMove += OnPreviewMouseMove;
		}
		else
		{
			element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
			element.PreviewMouseMove -= OnPreviewMouseMove;
		}
	}

	private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		_dragStartPosition = e.GetPosition(null);
	}

	private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || sender is not UIElement element)
		{
			return;
		}

		var currentPosition = e.GetPosition(null);
		var diff = _dragStartPosition - currentPosition;
		if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
			Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
		{
			return;
		}

		var task = FindDataContext<TaskItem>(e.OriginalSource as DependencyObject);
		if (task is null)
		{
			return;
		}

		DragDrop.DoDragDrop(element, task, DragDropEffects.Move);
	}

	private static void OnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not UIElement element)
		{
			return;
		}

		if (e.NewValue is not null)
		{
			element.AllowDrop = true;
			element.DragEnter += OnDragEnterOrOver;
			element.DragOver += OnDragEnterOrOver;
			element.Drop += OnDrop;
		}
		else
		{
			element.AllowDrop = false;
			element.DragEnter -= OnDragEnterOrOver;
			element.DragOver -= OnDragEnterOrOver;
			element.Drop -= OnDrop;
		}
	}

	/// <summary>
	/// ドラッグ中のデータがこの要素に受け入れ可能であることを明示する。
	/// WPFの既定動作では<see cref="DragEventArgs.Effects"/>を明示的に設定しない限りドロップが拒否扱いとなり
	/// <see cref="UIElement.Drop"/>イベント自体が発火しないため、DragEnter/DragOverでの明示が必須となる。
	/// </summary>
	private static void OnDragEnterOrOver(object sender, DragEventArgs e)
	{
		e.Effects = e.Data.GetDataPresent(typeof(TaskItem)) ? DragDropEffects.Move : DragDropEffects.None;
		e.Handled = true;
	}

	private static void OnDrop(object sender, DragEventArgs e)
	{
		if (sender is not DependencyObject container)
		{
			return;
		}

		if (!e.Data.GetDataPresent(typeof(TaskItem)) || e.Data.GetData(typeof(TaskItem)) is not TaskItem task)
		{
			return;
		}

		var command = GetDropCommand(container);
		var targetStatus = GetDropTargetStatus(container);
		var request = new MoveTaskRequest(task, targetStatus);

		if (command?.CanExecute(request) == true)
		{
			command.Execute(request);
		}
	}

	/// <summary>
	/// ビジュアルツリーを遡り、<see cref="FrameworkElement.DataContext"/> が指定した型に一致する最初の要素を探す。
	/// アイテムテンプレート内のどの要素からドラッグを開始しても、対応する <typeparamref name="T"/> を取得できるようにする。
	/// </summary>
	private static T? FindDataContext<T>(DependencyObject? source) where T : class
	{
		while (source is not null)
		{
			if (source is FrameworkElement { DataContext: T match })
			{
				return match;
			}

			source = VisualTreeHelper.GetParent(source);
		}

		return null;
	}
}
