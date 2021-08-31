using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorTools
{
    [TemplatePart(Name = nameof(ComponentCanvas), Type = typeof(Canvas))]
    [TemplatePart(Name = nameof(Handle), Type = typeof(Border))]
    [TemplatePart(Name = nameof(HandleTranslateTransform), Type = typeof(TranslateTransform))]
    public class ColorSlider : Control
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ColorSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(ColorSlider), new PropertyMetadata(0d, OnMinimumPropertyChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(ColorSlider), new PropertyMetadata(100d, OnMaximumPropertyChanged));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorSlider), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ColorSlider), new PropertyMetadata(null));

        public static readonly DependencyProperty AlphaBrushProperty =
            DependencyProperty.Register(nameof(AlphaBrush), typeof(Brush), typeof(ColorSlider), new PropertyMetadata(Brushes.Transparent));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(ColorSlider));

        private Canvas? _componentCanvas;

        private bool _isValueUpdating;
        private bool _isDragging;
        private BrushSource? _brushSource = BrushSource.Empty;

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public ColorSlider()
        {
            Loaded += OnLoaded;
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
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
                    _componentCanvas.MouseWheel -= OnComponentCanvasMouseWheel;
                    _componentCanvas.SizeChanged -= OnComponentCanvasSizeChanged;

                    _componentCanvas.Background = null;
                }

                _componentCanvas = value;

                if (_componentCanvas != null)
                {
                    _componentCanvas.MouseLeftButtonDown += OnComponentCanvasMouseLeftButtonDown;
                    _componentCanvas.MouseLeftButtonUp += OnComponentCanvasMouseLeftButtonUp;
                    _componentCanvas.MouseMove += OnComponentCanvasMouseMove;
                    _componentCanvas.MouseWheel += OnComponentCanvasMouseWheel;
                    _componentCanvas.SizeChanged += OnComponentCanvasSizeChanged;

                    _componentCanvas.Background = _brushSource?.Brush;
                }
            }
        }

        private Border? Handle { get; set; }

        private TranslateTransform? HandleTranslateTransform { get; set; }

        public void Update(double value)
        {
            SetCurrentValue(ValueProperty, value);

            UpdateBrushSource();
        }

        public void InitializeBrushSource(BrushSource brushSource)
        {
            _brushSource = brushSource;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ComponentCanvas = GetTemplateChild(nameof(ComponentCanvas)) as Canvas;
            Handle = GetTemplateChild(nameof(Handle)) as Border;
            HandleTranslateTransform = GetTemplateChild(nameof(HandleTranslateTransform)) as TranslateTransform;

            UpdateHandleCurrentPosition();
            UpdateBrushSource();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            UpdateHandleCurrentPosition();
            UpdateBrushSource();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            UpdateHandleCurrentPosition();
            UpdateBrushSource();
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

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider && args is { OldValue : double oldValue, })
            {
                slider.OnValueChanged(oldValue);
            }
        }

        private static void OnMinimumPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider)
            {
                slider.OnMinimumChanged();
            }
        }

        private static void OnMaximumPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ColorSlider slider)
            {
                slider.OnMaximumChanged();
            }
        }

        private void OnValueChanged(double oldValue)
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

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (oldValue == newValue)
                {
                    return;
                }

                UpdateHandleCurrentPosition();

                RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent));
            }
            finally
            {
                _isValueUpdating = false;
            }
        }

        private void OnMinimumChanged()
        {
            CoerceMaximum();
            CoerceValue();
        }

        private void OnMaximumChanged()
        {
            CoerceMinimum();
            CoerceValue();
        }

        private void OnComponentCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (ComponentCanvas == null)
            {
                return;
            }

            UpdateHandleNewPosition(args.GetPosition(ComponentCanvas).X);

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

            UpdateHandleNewPosition(args.GetPosition(ComponentCanvas).X);

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

        private void CoerceValue()
        {
            var value = Value;
            if (value < Minimum)
            {
                SetCurrentValue(ValueProperty, Minimum);
            }
            else if (value > Maximum)
            {
                SetCurrentValue(ValueProperty, Maximum);
            }
        }

        private void CoerceMinimum()
        {
            var maximum = Maximum;
            if (Minimum > maximum)
            {
                SetCurrentValue(MinimumProperty, maximum);
            }
        }

        private void CoerceMaximum()
        {
            var minimum = Minimum;
            if (Maximum < minimum)
            {
                SetCurrentValue(MaximumProperty, minimum);
            }
        }

        private void UpdateBrushSource()
        {
            if (!IsVisible || ComponentCanvas == null)
            {
                return;
            }

            _brushSource?.Update();
        }

        private void MoveHandle(int sign)
        {
            if (sign == 0)
            {
                return;
            }

            var isLargeStep = (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0;
            SetCurrentValue(ValueProperty, Value + sign * (isLargeStep ? 10 : 1));
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

            var (offset, width) = GetLayoutInfo();
            if (width <= 0)
            {
                return;
            }

            HandleTranslateTransform.X = ValueToHandlePosition(Value, offset, width, Minimum, Maximum);
        }

        private void UpdateHandleNewPosition(double mousePosition)
        {
            if (!IsVisible || ComponentCanvas == null || Handle == null || HandleTranslateTransform == null)
            {
                return;
            }

            var (offset, width) = GetLayoutInfo();
            if (width <= 0)
            {
                return;
            }

            HandleTranslateTransform.X = MousePositionToHandlePosition(mousePosition, offset, width);

            SetCurrentValue(ValueProperty, MousePositionToValue(mousePosition, offset, width, Minimum, Maximum));
        }

        private (double offset, double width) GetLayoutInfo()
        {
            if (ComponentCanvas == null || Handle == null)
            {
                return default;
            }

            var offset = Handle.Width / 2;
            var width = ComponentCanvas.ActualWidth - Handle.Width;
            return (offset, width);
        }

        private static double ValueToHandlePosition(double value, double offset, double width, double min, double max)
        {
            return offset + width * ((value - min) / (max - min));
        }

        private static double MousePositionToHandlePosition(double mousePosition, double offset, double width)
        {
            if (mousePosition < offset)
            {
                mousePosition = offset;
            }
            if (mousePosition > width + offset)
            {
                mousePosition = width + offset;
            }
            return mousePosition;
        }

        private static double MousePositionToValue(double mousePosition, double offset, double width, double min, double max)
        {
            var value = ((mousePosition - offset) / width) switch
            {
                < 0 => 0,
                > 1 => 1,
                var v => v,
            };
            return min + value * (max - min);
        }

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
    }
}
