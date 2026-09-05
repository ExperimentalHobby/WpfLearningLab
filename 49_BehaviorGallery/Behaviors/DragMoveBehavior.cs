using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace BehaviorGallery.Behaviors;

/// <summary>
/// アタッチした要素をCanvas内でマウスドラッグにより移動できるようにするビヘイビア。
/// </summary>
public class DragMoveBehavior : Behavior<FrameworkElement>
{
    private Point _lastPosition;
    private bool _isDragging;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        base.OnDetaching();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var canvas = GetCanvas();
        if (canvas == null)
        {
            return;
        }

        _isDragging = true;
        _lastPosition = e.GetPosition(canvas);
        AssociatedObject.CaptureMouse();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        var canvas = GetCanvas();
        if (canvas == null)
        {
            return;
        }

        var current = e.GetPosition(canvas);
        var delta = current - _lastPosition;

        var currentPosition = new Point(Canvas.GetLeft(AssociatedObject), Canvas.GetTop(AssociatedObject));
        var newPosition = DragMoveCalculator.CalculateNewPosition(currentPosition, delta);

        Canvas.SetLeft(AssociatedObject, newPosition.X);
        Canvas.SetTop(AssociatedObject, newPosition.Y);

        _lastPosition = current;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        AssociatedObject.ReleaseMouseCapture();
    }

    private Canvas? GetCanvas() => AssociatedObject.Parent as Canvas;
}
