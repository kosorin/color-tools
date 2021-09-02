using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorTools
{
    [TemplatePart(Name = nameof(ComponentCanvas), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(Handle), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(HandleTranslateTransform), Type = typeof(TranslateTransform))]
    public class ColorCanvas : ColorComponent
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Point), typeof(ColorCanvas),
                new FrameworkPropertyMetadata(new Point(0, 1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorCanvas), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ColorCanvas), new PropertyMetadata(null));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorCanvas), new PropertyMetadata(Brushes.Transparent));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Point>), typeof(ColorCanvas));

        private Canvas? _componentCanvas;

        private bool _isValueUpdating;
        private bool _isDragging;

        static ColorCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(typeof(ColorCanvas)));
        }

        public ColorCanvas()
        {
            Loaded += OnLoaded;
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public Point Value
        {
            get => (Point)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate?)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public Brush AlphaBrush
        {
            get => (Brush)GetValue(AlphaBrushProperty);
            set => SetValue(AlphaBrushProperty, value);
        }

        private Canvas? ComponentCanvas
        {
            get => _componentCanvas;
            set
            {
                if (_componentCanvas != null)
                {
                    _componentCanvas.MouseLeftButtonDown -= OnComponentCanvasMouseLeftButtonDown;
                    _componentCanvas.MouseLeftButtonUp -= OnComponentCanvasMouseLeftButtonUp;
                    _componentCanvas.MouseMove -= OnComponentCanvasMouseMove;
                    _componentCanvas.SizeChanged -= OnComponentCanvasSizeChanged;
                }

                _componentCanvas = value;

                if (_componentCanvas != null)
                {
                    _componentCanvas.MouseLeftButtonDown += OnComponentCanvasMouseLeftButtonDown;
                    _componentCanvas.MouseLeftButtonUp += OnComponentCanvasMouseLeftButtonUp;
                    _componentCanvas.MouseMove += OnComponentCanvasMouseMove;
                    _componentCanvas.SizeChanged += OnComponentCanvasSizeChanged;
                }
            }
        }

        private Canvas? Handle { get; set; }

        private TranslateTransform? HandleTranslateTransform { get; set; }

        public void Update(Point value)
        {
            Value = value;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ComponentCanvas = GetTemplateChild(nameof(ComponentCanvas)) as Canvas;
            Handle = GetTemplateChild(nameof(Handle)) as Canvas;
            HandleTranslateTransform = GetTemplateChild(nameof(HandleTranslateTransform)) as TranslateTransform;

            UpdateHandleCurrentPosition();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            UpdateHandleCurrentPosition();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            UpdateHandleCurrentPosition();
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorCanvas canvas && args is { OldValue : Point oldValue, })
            {
                canvas.OnValueChanged(oldValue);
            }
        }

        private void OnValueChanged(Point oldValue)
        {
            if (_isValueUpdating)
            {
                return;
            }

            try
            {
                _isValueUpdating = true;

                CoerceValue();

                var newValue = Value;

                if (oldValue == newValue)
                {
                    return;
                }

                UpdateHandleCurrentPosition();

                RaiseEvent(new RoutedPropertyChangedEventArgs<Point>(oldValue, newValue, ValueChangedEvent));
            }
            finally
            {
                _isValueUpdating = false;
            }
        }

        private void OnComponentCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (ComponentCanvas == null)
            {
                return;
            }

            UpdateHandleNewPosition(args.GetPosition(ComponentCanvas));

            _isDragging = true;
            ComponentCanvas.CaptureMouse();
            args.Handled = true;
        }

        private void OnComponentCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (ComponentCanvas == null)
            {
                return;
            }

            _isDragging = false;
            ComponentCanvas.ReleaseMouseCapture();
            args.Handled = true;
        }

        private void OnComponentCanvasMouseMove(object sender, MouseEventArgs args)
        {
            if (!_isDragging || args.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (ComponentCanvas == null)
            {
                return;
            }

            UpdateHandleNewPosition(args.GetPosition(ComponentCanvas));

            args.Handled = true;
        }

        private void OnComponentCanvasSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateHandleCurrentPosition();
        }

        private void CoerceValue()
        {
            var (x, y) = (Value.X, Value.Y);
            var coerced = false;

            if (x < 0)
            {
                x = 0;
                coerced = true;
            }
            else if (x > 1)
            {
                x = 1;
                coerced = true;
            }

            if (y < 0)
            {
                y = 0;
                coerced = true;
            }
            else if (y > 1)
            {
                y = 1;
                coerced = true;
            }

            if (coerced)
            {
                SetCurrentValue(ValueProperty, new Point(x, y));
            }
        }

        private void UpdateHandleCurrentPosition()
        {
            if (_isDragging)
            {
                return;
            }

            if (!IsVisible || ComponentCanvas == null || Handle == null || HandleTranslateTransform == null)
            {
                return;
            }

            var (offset, size) = GetLayoutInfo();
            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }

            var handlePosition = ValueToHandlePosition(Value, offset, size);
            HandleTranslateTransform.X = handlePosition.X;
            HandleTranslateTransform.Y = handlePosition.Y;
        }

        private void UpdateHandleNewPosition(Point mousePosition)
        {
            if (!IsVisible || ComponentCanvas == null || Handle == null || HandleTranslateTransform == null)
            {
                return;
            }

            var (offset, size) = GetLayoutInfo();
            if (size.Width <= 0 || size.Height <= 0)
            {
                return;
            }

            var handlePosition = MousePositionToHandlePosition(mousePosition, offset, size);
            HandleTranslateTransform.X = handlePosition.X;
            HandleTranslateTransform.Y = handlePosition.Y;

            SetCurrentValue(ValueProperty, HandlePositionToValue(handlePosition, offset, size));
        }

        private (Point offset, Size size) GetLayoutInfo()
        {
            if (ComponentCanvas == null || Handle == null)
            {
                return default;
            }

            var offset = new Point(Handle.Width / 2, Handle.Height / 2);
            var size = ComponentCanvas.RenderSize;
            return (offset, size);
        }

        private static Point ValueToHandlePosition(Point value, Point offset, Size size)
        {
            return new Point(value.X * size.Width - offset.X, (1 - value.Y) * size.Height - offset.Y);
        }

        private static Point MousePositionToHandlePosition(Point mousePosition, Point offset, Size size)
        {
            var (x, y) = (mousePosition.X, mousePosition.Y);

            if (x < 0)
            {
                x = 0;
            }
            else if (x > size.Width)
            {
                x = size.Width;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y > size.Height)
            {
                y = size.Height;
            }

            return new Point(x - offset.X, y - offset.Y);
        }

        private static Point HandlePositionToValue(Point handlePosition, Point offset, Size size)
        {
            return new Point((handlePosition.X + offset.X) / size.Width, 1 - (handlePosition.Y + offset.Y) / size.Height);
        }

        public event RoutedPropertyChangedEventHandler<Point> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
    }
}
