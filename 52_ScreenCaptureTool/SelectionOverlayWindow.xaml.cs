using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool;

/// <summary>
/// 仮想スクリーン全体を覆う半透明オーバーレイ。マウスドラッグで範囲を選択する。
/// </summary>
public partial class SelectionOverlayWindow : Window
{
	private Point _startPoint;
	private bool _isSelecting;

	/// <summary>
	/// 選択が確定した論理矩形(DIU)。キャンセル時は<see langword="null"/>のまま。
	/// </summary>
	public Rect? SelectedLogicalRect { get; private set; }

	public SelectionOverlayWindow()
	{
		InitializeComponent();
		Left = SystemParameters.VirtualScreenLeft;
		Top = SystemParameters.VirtualScreenTop;
		Width = SystemParameters.VirtualScreenWidth;
		Height = SystemParameters.VirtualScreenHeight;
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		_startPoint = e.GetPosition(RootCanvas);
		_isSelecting = true;

		Canvas.SetLeft(SelectionRectangle, _startPoint.X);
		Canvas.SetTop(SelectionRectangle, _startPoint.Y);
		SelectionRectangle.Width = 0;
		SelectionRectangle.Height = 0;
		SelectionRectangle.Visibility = Visibility.Visible;
	}

	private void Window_MouseMove(object sender, MouseEventArgs e)
	{
		if (!_isSelecting)
		{
			return;
		}

		var current = e.GetPosition(RootCanvas);
		var rect = MonitorDpiCoordinateConverter.Normalize(_startPoint, current);

		Canvas.SetLeft(SelectionRectangle, rect.Left);
		Canvas.SetTop(SelectionRectangle, rect.Top);
		SelectionRectangle.Width = rect.Width;
		SelectionRectangle.Height = rect.Height;
	}

	private void Window_MouseUp(object sender, MouseButtonEventArgs e)
	{
		if (!_isSelecting)
		{
			return;
		}

		_isSelecting = false;
		var current = e.GetPosition(RootCanvas);
		SelectedLogicalRect = MonitorDpiCoordinateConverter.Normalize(_startPoint, current);
		DialogResult = true;
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			DialogResult = false;
		}
	}
}
