using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Koda.ColorTools.Wpf.BrushSources;

namespace Koda.ColorTools.Wpf.Components
{
    [TemplatePart(Name = nameof(ComponentCanvas), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(Handle), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(HandleTranslateTransform), Type = typeof(TranslateTransform))]
    public abstract class ColorCanvas : ColorComponent
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Point), typeof(ColorCanvas),
                new FrameworkPropertyMetadata(new Point(0, 1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValuePropertyChanged, OnCoerceValue));

        public static readonly DependencyProperty CanvasBrushSourceProperty =
            DependencyProperty.Register(nameof(CanvasBrushSource), typeof(BrushSource), typeof(ColorCanvas), new PropertyMetadata(null));

        private Canvas? _componentCanvas;

        private bool _isDragging;

        static ColorCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(typeof(ColorCanvas)));
        }

        protected ColorCanvas()
        {
            SetCurrentValue(CanvasBrushSourceProperty, new SolidColorBrushSource());

            Loaded += OnLoaded;
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public Point Value
        {
            get => (Point)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public BrushSource CanvasBrushSource
        {
            get => (BrushSource)GetValue(CanvasBrushSourceProperty);
            set => SetValue(CanvasBrushSourceProperty, value);
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

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorCanvas canvas)
            {
                canvas.OnValueChanged();
            }
        }

        private static object OnCoerceValue(DependencyObject sender, object baseValue)
        {
            if (baseValue is not Point value)
            {
                return new Point(0, 0);
            }

            return new Point(value.X switch
            {
                < 0 => 0,
                > 1 => 1,
                var x => x,
            }, value.Y switch
            {
                < 0 => 0,
                > 1 => 1,
                var y => y,
            });
        }

        private void OnValueChanged()
        {
            UpdateHandleCurrentPosition();

            TryUpdatePicker();
        }

        private void OnComponentCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (ComponentCanvas == null)
            {
                return;
            }

            Picker?.BeginUpdate();

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

            Picker?.EndUpdate();

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
    }
}
