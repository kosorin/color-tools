using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColorTools.BrushSources;

namespace ColorTools.Components
{
    [TemplatePart(Name = nameof(ComponentCanvas), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(Handle), Type = typeof(Border))]
    [TemplatePart(Name = nameof(HandleTranslateTransform), Type = typeof(TranslateTransform))]
    public abstract class ColorSlider : ColorComponent
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ColorSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValuePropertyChanged, OnCoerceValue));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ColorSlider), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty CanvasBrushSourceProperty =
            DependencyProperty.Register(nameof(CanvasBrushSource), typeof(BrushSource), typeof(ColorSlider), new PropertyMetadata(null));

        private Canvas? _componentCanvas;

        private bool _isDragging;

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        protected ColorSlider()
        {
            SetCurrentValue(CanvasBrushSourceProperty, new SolidColorBrushSource());
            
            Loaded += OnLoaded;
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
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
                    _componentCanvas.MouseWheel -= OnComponentCanvasMouseWheel;
                    _componentCanvas.SizeChanged -= OnComponentCanvasSizeChanged;
                }

                _componentCanvas = value;

                if (_componentCanvas != null)
                {
                    _componentCanvas.MouseLeftButtonDown += OnComponentCanvasMouseLeftButtonDown;
                    _componentCanvas.MouseLeftButtonUp += OnComponentCanvasMouseLeftButtonUp;
                    _componentCanvas.MouseMove += OnComponentCanvasMouseMove;
                    _componentCanvas.MouseWheel += OnComponentCanvasMouseWheel;
                    _componentCanvas.SizeChanged += OnComponentCanvasSizeChanged;
                }
            }
        }

        private Border? Handle { get; set; }

        private TranslateTransform? HandleTranslateTransform { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ComponentCanvas = GetTemplateChild(nameof(ComponentCanvas)) as Canvas;
            Handle = GetTemplateChild(nameof(Handle)) as Border;
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

        protected override void OnKeyDown(KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Left:
                case Key.PageDown:
                    MoveHandle(-1);
                    break;
                case Key.Right:
                case Key.PageUp:
                    MoveHandle(1);
                    break;
            }
        }

        private static void OnValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider)
            {
                slider.OnValueChanged();
            }
        }

        private static object OnCoerceValue(DependencyObject sender, object baseValue)
        {
            return baseValue switch
            {
                double and < 0 => 0,
                double and > 1 => 1,
                double value => value,
                _ => 0d,
            };
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

            UpdateHandleNewPosition(GetMousePosition(args.GetPosition(ComponentCanvas)));

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

            UpdateHandleNewPosition(GetMousePosition(args.GetPosition(ComponentCanvas)));

            args.Handled = true;
        }

        private void OnComponentCanvasMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (_isDragging)
            {
                return;
            }

            MoveHandle(Math.Sign(args.Delta));
        }

        private void OnComponentCanvasSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateHandleCurrentPosition();
        }

        private void MoveHandle(int sign)
        {
            if (sign == 0)
            {
                return;
            }

            var isLargeStep = (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None;
            SetCurrentValue(ValueProperty, Value + sign * (isLargeStep ? 0.1 : 0.01));
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
            if (size <= 0)
            {
                return;
            }

            SetHandlePosition(ValueToHandlePosition(Value, offset, size, Orientation == Orientation.Vertical));
        }

        private void UpdateHandleNewPosition(double mousePosition)
        {
            if (!IsVisible || ComponentCanvas == null || Handle == null || HandleTranslateTransform == null)
            {
                return;
            }

            var (offset, size) = GetLayoutInfo();
            if (size <= 0)
            {
                return;
            }

            SetHandlePosition(MousePositionToHandlePosition(mousePosition, offset, size));

            SetCurrentValue(ValueProperty, MousePositionToValue(mousePosition, offset, size, Orientation == Orientation.Vertical));
        }

        private (double offset, double size) GetLayoutInfo()
        {
            if (ComponentCanvas == null || Handle == null)
            {
                return default;
            }

            return Orientation == Orientation.Vertical
                ? (Handle.Height / 2, ComponentCanvas.ActualHeight - Handle.Height)
                : (Handle.Width / 2, ComponentCanvas.ActualWidth - Handle.Width);
        }

        private double GetMousePosition(Point mousePosition)
        {
            return Orientation == Orientation.Vertical ? mousePosition.Y : mousePosition.X;
        }

        private void SetHandlePosition(double handlePosition)
        {
            if (HandleTranslateTransform == null)
            {
                return;
            }

            if (Orientation == Orientation.Vertical)
            {
                HandleTranslateTransform.Y = handlePosition;
            }
            else
            {
                HandleTranslateTransform.X = handlePosition;
            }
        }

        private static double ValueToHandlePosition(double value, double offset, double size, bool invert)
        {
            var position = invert
                ? 1 - value
                : value;
            return offset + size * position;
        }

        private static double MousePositionToHandlePosition(double mousePosition, double offset, double size)
        {
            if (mousePosition < offset)
            {
                mousePosition = offset;
            }
            if (mousePosition > size + offset)
            {
                mousePosition = size + offset;
            }
            return mousePosition;
        }

        private static double MousePositionToValue(double mousePosition, double offset, double size, bool invert)
        {
            var value = ((mousePosition - offset) / size) switch
            {
                < 0 => 0,
                > 1 => 1,
                var v => v,
            };

            return invert
                ? 1 - value
                : value;
        }
    }
}